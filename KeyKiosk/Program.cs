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

            var connectionString = builder.Configuration.GetConnectionString("DefaultConnection") ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");

            void DbOptions(DbContextOptionsBuilder options)
            {
                options.UseSqlite(connectionString);
                options.LogTo(Console.WriteLine, minimumLevel: LogLevel.Information);
                //options.LogTo(Console.WriteLine, minimumLevel: LogLevel.Debug);
                options.EnableSensitiveDataLogging();
            }

            builder.Services.AddDbContext<ApplicationDbContext>(DbOptions);

            if (!EF.IsDesignTime)
            {
                var options = new DbContextOptionsBuilder<ApplicationDbContext>();
                DbOptions(options);
                var ctx = new ApplicationDbContext(options.Options);

                if (ctx.Database.GetPendingMigrations().Count() > 0)
                {
                    ctx.Database.Migrate();
                }

                var cfgs = builder.Configuration.GetDrawerConfigs() ?? throw new InvalidOperationException("Configuration section 'Drawers' not found.");

                var dbCount = ctx.Drawers.Count();

                if (cfgs.Count == 0)
                {
                    throw new InvalidOperationException("Configuration section 'Drawers' has no entries.");
                }
                else if (dbCount == 0)
                {
                    for (int i = 1; i <= cfgs.Count; i++)
                    {
                        int li = i;
                        ctx.Drawers.Add(new() { Id = li, Occupied = false });
                    }
                    ctx.SaveChanges();
                }
                else if (dbCount != cfgs.Count)
                {
                    throw new InvalidOperationException("Configuration section 'Drawers' entry count doesn't match database. Please delete/flush database entries.");
                }


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

            app.UseStaticFiles();
            app.UseAntiforgery();

            app.MapRazorComponents<App>()
                .AddInteractiveServerRenderMode();

            app.Run();
        }
    }
}
