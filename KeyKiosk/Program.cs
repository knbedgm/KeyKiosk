using KeyKiosk.Components;
using KeyKiosk.Data;
using KeyKiosk.Services;
using Microsoft.EntityFrameworkCore;
using MudBlazor.Services;
using QuestPDF.Infrastructure;

namespace KeyKiosk
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            QuestPDF.Settings.License = LicenseType.Community;

            // Add services to the container.
            builder.Services.AddRazorComponents()
                .AddInteractiveServerComponents();

			builder.Services.AddMudServices(config =>
            {
                config.SnackbarConfiguration.PositionClass = MudBlazor.Defaults.Classes.Position.BottomRight;
            });

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
                builder.RegiserAppServices();

                // Initialize drawer entries in database
                var options = new DbContextOptionsBuilder<ApplicationDbContext>();
                DbOptions(options);
                var ctx = new ApplicationDbContext(options.Options);

                if (ctx.Database.GetPendingMigrations().Count() > 0)
                {
                    ctx.Database.Migrate();
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