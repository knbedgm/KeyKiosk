using KeyKiosk.Data;
using KeyKiosk.Services;
using KeyKiosk.Services.Auth;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.Extensions.DependencyInjection;
using System.Runtime.CompilerServices;

namespace KeyKiosk.Data.Interceptors
{
    public class WorkOrderAuditInterceptor : SaveChangesInterceptor
    {
        private readonly IServiceProvider _rootProvider;

        // Holds pending log actions per DbContext instance
        private static readonly ConditionalWeakTable<DbContext, List<Func<IServiceProvider, Task>>> _pendingLogs
            = new();

        public WorkOrderAuditInterceptor(IServiceProvider rootProvider)
        {
            _rootProvider = rootProvider;
        }

        public override ValueTask<InterceptionResult<int>> SavingChangesAsync(
            DbContextEventData eventData,
            InterceptionResult<int> result,
            CancellationToken cancellationToken = default)
        {
            var context = eventData.Context;
            if (context == null) return new(result);

            var pending = _pendingLogs.GetOrCreateValue(context);
            pending.Clear();

            // --- WorkOrder changes ---
            foreach (var entry in context.ChangeTracker.Entries<WorkOrder>())
            {
                if (entry.State == EntityState.Added)
                {
                    pending.Add(sp => LogAsync(sp, l => l.LogCreatedAsync(entry.Entity, GetUser(sp))));
                }
                else if (entry.State == EntityState.Modified)
                {
                    var origStatus = entry.OriginalValues.GetValue<WorkOrderStatusType>(nameof(WorkOrder.Status));
                    var newStatus = entry.CurrentValues.GetValue<WorkOrderStatusType>(nameof(WorkOrder.Status));

                    var origCustomer = entry.OriginalValues.GetValue<string>(nameof(WorkOrder.CustomerName)) ?? "";
                    var newCustomer = entry.CurrentValues.GetValue<string>(nameof(WorkOrder.CustomerName)) ?? "";

                    var origPlate = entry.OriginalValues.GetValue<string>(nameof(WorkOrder.VehiclePlate)) ?? "";
                    var newPlate = entry.CurrentValues.GetValue<string>(nameof(WorkOrder.VehiclePlate)) ?? "";

                    var origDetails = entry.OriginalValues.GetValue<string>(nameof(WorkOrder.Details)) ?? "";
                    var newDetails = entry.CurrentValues.GetValue<string>(nameof(WorkOrder.Details)) ?? "";

                    if (!Equals(origStatus, newStatus))
                        pending.Add(sp => LogAsync(sp, l => l.LogStatusChangedAsync(entry.Entity, GetUser(sp))));

                    if (!Equals(origCustomer, newCustomer) ||
                        !Equals(origPlate, newPlate) ||
                        !Equals(origDetails, newDetails))
                        pending.Add(sp => LogAsync(sp, l => l.LogDetailsChangedAsync(entry.Entity, GetUser(sp))));
                }
            }

            // --- WorkOrderTask changes ---
            foreach (var entry in context.ChangeTracker.Entries<WorkOrderTask>())
            {
                if (entry.State == EntityState.Added)
                {
                    pending.Add(sp => LogAsync(sp, l => l.LogTaskAddedAsync(entry.Entity, GetUser(sp))));
                }
                else if (entry.State == EntityState.Modified)
                {
                    var origStatus = entry.OriginalValues.GetValue<WorkOrderTaskStatusType>(nameof(WorkOrderTask.Status));
                    var newStatus = entry.CurrentValues.GetValue<WorkOrderTaskStatusType>(nameof(WorkOrderTask.Status));

                    var origDetails = entry.OriginalValues.GetValue<string>(nameof(WorkOrderTask.Details)) ?? "";
                    var newDetails = entry.CurrentValues.GetValue<string>(nameof(WorkOrderTask.Details)) ?? "";

                    var origCost = entry.OriginalValues.GetValue<int>(nameof(WorkOrderTask.CostCents));
                    var newCost = entry.CurrentValues.GetValue<int>(nameof(WorkOrderTask.CostCents));

                    if (!Equals(origStatus, newStatus))
                        pending.Add(sp => LogAsync(sp, l => l.LogTaskStatusChangedAsync(entry.Entity, GetUser(sp))));

                    if (!Equals(origDetails, newDetails) || origCost != newCost)
                        pending.Add(sp => LogAsync(sp, l => l.LogTaskDetailsChangedAsync(entry.Entity, GetUser(sp))));
                }
                else if (entry.State == EntityState.Deleted)
                {
                    pending.Add(sp => LogAsync(sp, l => l.LogTaskRemovedAsync(entry.Entity, GetUser(sp))));
                }
            }

            return new(result);
        }

        public override async ValueTask<int> SavedChangesAsync(
            SaveChangesCompletedEventData eventData,
            int result,
            CancellationToken cancellationToken = default)
        {
            var context = eventData.Context;
            if (context == null) return result;

            if (_pendingLogs.TryGetValue(context, out var pending) && pending.Count > 0)
            {
                using var scope = _rootProvider.CreateScope();
                foreach (var work in pending)
                {
                    await work(scope.ServiceProvider);
                }
                pending.Clear();
            }

            return result;
        }

        // Helper: resolve user inline
        private static User GetUser(IServiceProvider sp)
        {
            var db = sp.GetRequiredService<ApplicationDbContext>();
			var auth = sp.GetRequiredService<AppAuthenticationStateProvider>();
			return auth.CurrentSession?.User ?? db.Users.First(); // fallback: first user in DB
		}

        // Helper: run a log action
        private static async Task LogAsync(IServiceProvider sp, Func<WorkOrderLogService, Task> action)
        {
            var logService = sp.GetRequiredService<WorkOrderLogService>();
            await action(logService);
        }
    }
}
