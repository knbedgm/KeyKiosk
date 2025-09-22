using KeyKiosk.Data;

namespace KeyKiosk.Services
{
    public class DrawerService
    {
        List<DrawerConfig> DrawerIOConfig;
        IPhysicalDrawerController DrawerController;
        ApplicationDbContext dbContext;
        UserSessionService userSessionService;
        List<Drawer> drawers;
        public DrawerService(IEnumerable<DrawerConfig> conf, IPhysicalDrawerController DrawerController, ApplicationDbContext dbContext, UserSessionService userSessionService)
        {
            DrawerIOConfig = new List<DrawerConfig>(conf);
            this.DrawerController = DrawerController;
            this.dbContext = dbContext;
            this.userSessionService = userSessionService;
            
            drawers = new List<Drawer>();
            foreach (var d in dbContext.Drawers)
            {
                Console.WriteLine($"Loading {d} @ {d.Id}");
                drawers.Add(new() { db= d, config= DrawerIOConfig[d.Id-1] });
            }
        }

        public async Task Open(int id)
        {
            //TODO: make service scoped to access use / database for audit
            dbContext.DrawerLog.Add(new() { DateTime = DateTime.Now, EventType = DrawerLogEventType.Open, DrawerId = id, User = userSessionService.User!});
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
