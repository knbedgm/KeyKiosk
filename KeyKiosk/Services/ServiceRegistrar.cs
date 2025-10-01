using KeyKiosk.Data;

namespace KeyKiosk.Services
{
	public static class ServiceRegistrar
	{
		public static void RegiserAppServices(this WebApplicationBuilder builder)
		{
			// Singleton services exist for the entire app and every connection receieves the same one
			// Scoped services exist for the length of a connection and every connection recieves a different instance

			builder.Services.AddSingleton<SerialTest>();
			builder.Services.AddScoped<ScopedTest>();
			builder.Services.AddScoped<KioskUserSessionService>();
			builder.Services.AddScoped<KioskNavAuthService>();
			builder.Services.AddScoped<WorkOrderService>();
			builder.Services.AddScoped<WorkOrderTaskTemplateService>();

			// Drawer Serial Interface Service
			//string port = builder.Configuration.GetRequiredSection("DrawerSerialPort").Value ?? throw new InvalidOperationException("Configuration string 'DrawerSerialPort' not found.");
			//builder.Services.AddSingleton<IPhysicalDrawerController>(new DenkoviDrawerController(port));
			builder.Services.AddSingleton<IPhysicalDrawerController, TestConsoleDrawerController>();

			// Drawer High-level control service
			var drawerConfigs = builder.Configuration.GetDrawerConfigs();
			builder.Services.AddScoped<DrawerService>((IServiceProvider svc) =>
			{
				var db = svc.GetRequiredService<ApplicationDbContext>();
				var controller = svc.GetRequiredService<IPhysicalDrawerController>();
				var users = svc.GetRequiredService<KioskUserSessionService>();
				return new(drawerConfigs, controller, db, users);
			});
		}
	}
}
