using KeyKiosk.Data;
using KeyKiosk.Services.Auth;

namespace KeyKiosk.Services
{
    public static class ServiceRegistrar
    {
        public static void RegiserAppServices(this WebApplicationBuilder builder)
        {
            // Singleton services exist for the entire app and every connection receieves the same one
            // Scoped services exist for the length of a connection and every connection recieves a different instance

			builder.Services.AddSingleton<SerialTest>();
            builder.Services.AddSingleton<HSimService>();
            builder.Services.AddSingleton<IRFIDReader>(sp => sp.GetRequiredService<HSimService>());
            //builder.Services.AddSingleton<IRFIDReader, PCSCReaderService>();
			builder.Services.AddScoped<ScopedTest>();
            builder.Services.AddScoped<KioskNavAuthService>();
            builder.Services.AddScoped<WorkOrderService>();
            builder.Services.AddScoped<WorkOrderTaskService>();
            builder.Services.AddScoped<WorkOrderTaskTemplateService>();
            builder.Services.AddScoped<PDFService>();
            builder.Services.AddScoped<WorkOrderPartService>();
            builder.Services.AddScoped<PartTemplateService>();
            builder.Services.AddScoped<PreviewDownloadService>();

            builder.Services.AddScoped<WorkOrderLogService>();
            builder.Services.AddScoped<PartTemplateService>();
            builder.Services.AddScoped<PreviewDownloadService>();
            builder.Services.AddScoped<PDFService>();
            builder.Services.AddScoped<UserService>();

            // Drawer Serial Interface Service
            //string port = builder.Configuration.GetRequiredSection("DrawerSerialPort").Value ?? throw new InvalidOperationException("Configuration string 'DrawerSerialPort' not found.");
            //builder.Services.AddSingleton<IPhysicalDrawerController>(new DenkoviDrawerController(port));
            //builder.Services.AddSingleton<IPhysicalDrawerController, TestConsoleDrawerController>();
            builder.Services.AddSingleton<IPhysicalDrawerController>(sp => sp.GetRequiredService<HSimService>());

            // Drawer High-level control service
            var drawerConfigs = builder.Configuration.GetDrawerConfigs();
            builder.Services.AddScoped<DrawerService>((IServiceProvider svc) =>
            {
                var db = svc.GetRequiredService<ApplicationDbContext>();
                var controller = svc.GetRequiredService<IPhysicalDrawerController>();
                var users = svc.GetRequiredService<AppAuthenticationStateProvider>();
                return new(drawerConfigs, controller, db, users);
            });
        }
    }
}
