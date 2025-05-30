﻿using KeyKiosk.Data;

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
            var drawer = dbContext.Drawers.First(d => d.Id == id);
            if (drawer != null)
            {
                drawer.Occupied = !drawer.Occupied;
            }
            //dbContext.Drawers.Add(new());
            dbContext.SaveChanges();
            await DrawerController.Open(id);
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
            public required  DrawerConfig config { private get; init; }
            public required Data.Drawer db { private get; init; }
            public int Id { get => db.Id; }
            public string Name { get => config.Name; }
            public string? CurrentRONumber { get => db.CurrentRONumber; }
            public bool Occupied { get => db.Occupied; }
        }
    }
}
