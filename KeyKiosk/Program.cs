using KeyKiosk.Components;
using KeyKiosk.Data;
using KeyKiosk.Services;
using Microsoft.EntityFrameworkCore;

namespace KeyKiosk
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.
            builder.Services.AddRazorComponents()
                .AddInteractiveServerComponents();

            builder.Services.AddSingleton<SerialTest>();
            builder.Services.AddScoped<ScopedTest>();
            builder.Services.AddScoped<UserSessionService>();
            builder.Services.AddScoped<NavAuthService>();

            var connectionString = builder.Configuration.GetConnectionString("DefaultConnection") ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");

            void DbOptions(DbContextOptionsBuilder options)
            {
                //options.UseSqlite(connectionString);
                options.UseNpgsql(connectionString);
                //options.LogTo(Console.WriteLine, minimumLevel: LogLevel.Information);
                //options.LogTo(Console.WriteLine, minimumLevel: LogLevel.Debug);
                options.EnableSensitiveDataLogging();
            }

            builder.Services.AddDbContext<ApplicationDbContext>(DbOptions);

            // prevent config issues from blocking `dotnet ef migration` commands
            if (!EF.IsDesignTime)
            {
                // Drawer Serial Interface Service
                //string port = builder.Configuration.GetRequiredSection("DrawerSerialPort").Value ?? throw new InvalidOperationException("Configuration string 'DrawerSerialPort' not found.");
                //builder.Services.AddSingleton<IPhysicalDrawerController>(new DenkoviDrawerController(port));
                builder.Services.AddSingleton<IPhysicalDrawerController, TestConsoleDrawerController>();

                // Drawer High-level control service
                var drawerConfigs = builder.Configuration.GetDrawerConfigs() ?? throw new InvalidOperationException("Configuration section 'Drawers' not found.");
                builder.Services.AddScoped<DrawerService>((IServiceProvider svc) =>
                {
                    var db = svc.GetRequiredService<ApplicationDbContext>();
                    var controller = svc.GetRequiredService<IPhysicalDrawerController>();
                    var users = svc.GetRequiredService<UserSessionService>();
                    return new(drawerConfigs, controller, db, users);
                });


                // Initialize drawer entries in database
                var options = new DbContextOptionsBuilder<ApplicationDbContext>();
                DbOptions(options);
                var ctx = new ApplicationDbContext(options.Options);

                if (ctx.Database.GetPendingMigrations().Count() > 0)
                {
                    ctx.Database.Migrate();
                }


                var dbCount = ctx.Drawers.Count();

                if (drawerConfigs.Count == 0)
                {
                    throw new InvalidOperationException("Configuration section 'Drawers' has no entries.");
                }
                else if (dbCount == 0)
                {
                    for (int i = 1; i <= drawerConfigs.Count; i++)
                    {
                        ctx.Drawers.Add(new() { Id = i, Occupied = false});
                    }
                    ctx.SaveChanges();
                }
                else if (dbCount != drawerConfigs.Count)
                {
                    throw new InvalidOperationException("Configuration section 'Drawers' entry count doesn't match database. Please delete/flush database entries.");
                }


                // create first user if none exist
                if (ctx.Users.Count() == 0)
                {
                    ctx.Users.Add(new() { Id = 1, Name = "DefaultAdmin", Pin = "555555", UserType = UserType.Admin });
                    ctx.SaveChanges();
                }
            }

            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (!app.Environment.IsDevelopment())
            {
                app.UseExceptionHandler("/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                //app.UseHsts();
            }

            //app.UseHttpsRedirection();

            app.MapStaticAssets();
            app.UseAntiforgery();

            app.MapRazorComponents<App>()
                .AddInteractiveServerRenderMode();

            app.Run();
        }
    }
}
