using KeyKiosk.Data;
using KeyKiosk.Services.Auth;
using System.Collections;

namespace KeyKiosk.Services
{
    public class DrawerService
    {
        IList<DrawerConfig> DrawerIOConfig;
        IPhysicalDrawerController DrawerController;
        ApplicationDbContext dbContext;
		AppAuthenticationStateProvider userSessionService;
        List<Drawer> drawers;
        public DrawerService(IList<DrawerConfig> DrawerIOConfig, IPhysicalDrawerController DrawerController, ApplicationDbContext dbContext, AppAuthenticationStateProvider userSessionService)
        {
            this.DrawerIOConfig = DrawerIOConfig;
            this.DrawerController = DrawerController;
            this.dbContext = dbContext;
            this.userSessionService = userSessionService;

            this.initDb(DrawerIOConfig);

            drawers = new List<Drawer>();

            var dbDrawers = from drawer in dbContext.Drawers orderby drawer.Id select drawer;
            foreach (var d in dbDrawers)
            {
                //Console.WriteLine($"Loading {d} @ {d.Id}");
                drawers.Add(new() { db= d, config= DrawerIOConfig[d.Id-1] });
            }
        }

        public async Task Open(int id)
        {
            //TODO: make service scoped to access use / database for audit
            dbContext.DrawerLog.Add(new() { DateTime = DateTime.Now, EventType = DrawerLogEventType.Open, DrawerId = id, User = userSessionService.CurrentSession!.User});
            var drawer = drawers.First(d => d.Id == id);
            if (drawer == null) throw new ArgumentException($"Unable to find drawer with id ${id}", "id");

            drawer.db.Occupied = !drawer.Occupied;
            dbContext.SaveChanges();
            await DrawerController.Open(drawer.config.RelayIndex);
        }

        public async Task OpenAll()
        {
            await DrawerController.OpenAll();
        }

        public IEnumerable<Drawer> GetDrawers()
        {
            return drawers;
        }

        private void initDb(IList<DrawerConfig> drawerConfigs)
        {
			var dbCount = dbContext.Drawers.Count();

			if (drawerConfigs.Count() == 0)
			{
				throw new InvalidOperationException("Configuration section 'Drawers' has no entries.");
			}
			else if (dbCount == 0)
			{
				for (int i = 1; i <= drawerConfigs.Count; i++)
				{
					dbContext.Drawers.Add(new() { Id = i, Occupied = false });
				}
				dbContext.SaveChanges();
			}
			else if (dbCount != drawerConfigs.Count)
			{
				throw new InvalidOperationException("Configuration section 'Drawers' entry count doesn't match database. Please delete/flush database entries.");
			}
		}

        public class Drawer
        {
            public required  DrawerConfig config { internal get; init; }
            public required Data.Drawer db { internal get; init; }
            public int Id { get => db.Id; }
            public string Name { get => config.Name; }
            public string? CurrentRONumber { get => db.CurrentRONumber; }
            public bool Occupied { get => db.Occupied; }
        }
    }
}
