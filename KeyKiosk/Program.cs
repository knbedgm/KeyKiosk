using KeyKiosk.Components;
using KeyKiosk.Data;
using KeyKiosk.Data.Interceptors;
using KeyKiosk.Services;
using KeyKiosk.Services.Auth;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Internal;
using MudBlazor.Services;
using QuestPDF.Infrastructure;
using System.DirectoryServices;
using System.Security.Claims;

namespace KeyKiosk
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            QuestPDF.Settings.License = LicenseType.Community;

            AddAppAuth(builder);

            // Add services to the container.
            builder.Services.AddRazorComponents()
                .AddInteractiveServerComponents();

            builder.Services.AddMudServices(config =>
            {
                config.SnackbarConfiguration.PositionClass = MudBlazor.Defaults.Classes.Position.BottomRight;
            });

            var connectionString = builder.Configuration.GetConnectionString("DefaultConnection") ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");

            //pdf generation
            void DbOptions(DbContextOptionsBuilder options)
            {
                //options.UseSqlite(connectionString);
                options.UseNpgsql(connectionString);
                //options.LogTo(Console.WriteLine, minimumLevel: LogLevel.Information);
                //options.LogTo(Console.WriteLine, minimumLevel: LogLevel.Debug);
                options.EnableSensitiveDataLogging();
            }

            builder.Services.AddDbContext<ApplicationDbContext>(DbOptions);

            builder.Services.AddScoped<WorkOrderLogService>();
            builder.Services.AddScoped<WorkOrderAuditService>();
            builder.Services.AddScoped<WorkOrderAuditInterceptor>();

            // DbContext for DI with interceptor
            builder.Services.AddDbContext<ApplicationDbContext>((sp, options) =>
            {
                options.UseNpgsql(connectionString);
                options.EnableSensitiveDataLogging();
                options.AddInterceptors(sp.GetRequiredService<WorkOrderAuditInterceptor>());
            });


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
                    //ctx.Users.Add(new() { Id = 1, Name = "DefaultAdmin", Pin = "555555", UserType = UserType.Admin, DesktopLogin = new UserDesktopLogin {Username = "admin", HashedPassword =  } });
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

        private static void AddAppAuth(WebApplicationBuilder builder)
        {
            //builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
            //	.AddCookie(CookieAuthenticationDefaults.AuthenticationScheme, options =>
            //	{
            //		options.Cookie.HttpOnly = true;
            //		options.Cookie.SameSite = SameSiteMode.Strict;
            //		options.Cookie.SecurePolicy = CookieSecurePolicy.SameAsRequest; // Change maybe?
            //		//options.LoginPath = "/auth/login";
            //		//options.LogoutPath = "/auth/logout";
            //		options.SlidingExpiration = true; // = false; // we'll handle expiry server-side

            //		options.Events = new CookieAuthenticationEvents
            //		{
            //			/// OnValidatePrincipal = Possibly add session invalidation stuff here

            //			OnRedirectToLogin = context =>
            //			{
            //				var path = context.Request.Path.ToString().ToLower();

            //				// If user was on kiosk pages or a kiosk session, send to kiosk login
            //				if (path.StartsWith("/kiosk"))
            //					context.Response.Redirect("/kiosk/");
            //				else
            //					context.Response.Redirect("/admin/login");

            //				return Task.CompletedTask;
            //			},

            //			OnRedirectToLogout = context =>
            //			{
            //				var path = context.Request.Path.ToString().ToLower();

            //				// If user was on kiosk pages or a kiosk session, send to kiosk login
            //				if (path.StartsWith("/kiosk"))
            //					context.Response.Redirect("/kiosk/");
            //				else
            //					context.Response.Redirect("/");

            //				return Task.CompletedTask;
            //			},
            //		};

            //	});

            builder.Services.AddAuthentication().AddScheme<SessionTokenAuthenticationSchemeOptions, SessionTokenAuthenticationSchemeHandler>("CookieSessionId", options => { });

            builder.Services.AddCascadingAuthenticationState();
            builder.Services.AddSingleton<AppAuthenticationSessionStorage>();
            builder.Services.AddScoped<UserService>();
            builder.Services.AddScoped<AppAuthenticationSessionAccessor>();
            builder.Services.AddScoped<AppAuthenticationStateProvider>();
            builder.Services.AddScoped<AuthenticationStateProvider>(s => s.GetRequiredService<AppAuthenticationStateProvider>());

            builder.Services.AddAuthorizationBuilder()
                .AddPolicy("Desktop", policy =>
                    policy.RequireClaim("LoginType", "Desktop"))
                .AddPolicy("Kiosk", policy =>
                    policy.RequireClaim("LoginType", "Kiosk"))
                .AddPolicy("RoleAdmin", policy =>
                    policy.RequireClaim(ClaimTypes.Role, "Admin"))
                .AddPolicy("RoleManager", policy =>
                    policy.RequireClaim(ClaimTypes.Role, ["Admin", "Manager"]));
        }
    }
}