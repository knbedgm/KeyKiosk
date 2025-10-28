using KeyKiosk.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using System.Collections;

namespace KeyKiosk.Data
{
    public class ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : DbContext(options)
    {
        public DbSet<User> Users { get; set; }
        public DbSet<Drawer> Drawers { get; set; }
        public DbSet<WorkOrder> WorkOrders { get; set; }
        public DbSet<WorkOrderTask> WorkOrderTasks { get; set; }
        public DbSet<WorkOrderTaskTemplate> WorkOrderTaskTemplates { get; set; }

        public DbSet<DrawerLogEvent> DrawerLog { get; set; }
        public DbSet<UserLogEvent> UserLog { get; set; }

        public DbSet<WorkOrderLogEvent> WorkOrderLog { get; set; }
        public DbSet<WorkOrderLogEvent> LogEvent { get; set; } = null!;
        public DbSet<WorkOrderLogsRaw> WorkOrderLogsRaw { get; set; } = null!;


        //mapping for WorkOrderLogEvent and its subclasses is done in WorkOrderLogEventEntityTypeConfiguration
        /// <summary>
        /// prevents EF Core conventions from generating the wrong SQL 
        /// and is the reason your app can reliably read from and write to the existing WorkOrderLogs table without changing the Neon schema
        /// </summary>
        protected override void OnModelCreating(ModelBuilder modelBuilder)
		{
            base.OnModelCreating(modelBuilder);
            new WorkOrderLogEventEntityTypeConfiguration().Configure(modelBuilder.Entity<WorkOrderLogEvent>());
            modelBuilder.Entity<WorkOrderLogEvent>().ToTable("workorderlog");

            modelBuilder.Entity<WorkOrderLogsRaw>(builder =>
            {
                builder.ToTable("WorkOrderLog", "public");

                builder.HasKey(e => e.ID);
                builder.Property(e => e.ID).HasColumnName("ID");

                builder.Property(e => e.DateTime).HasColumnName("DateTime");
                builder.Property(e => e.UserId).HasColumnName("UserId");
                builder.Property(e => e.UserName).HasColumnName("UserName");
                builder.Property(e => e.workOrderId).HasColumnName("workOrderId");
                builder.Property(e => e.EventType).HasColumnName("EventType");

                builder.Property(e => e.CustomerName).HasColumnName("CustomerName");
                builder.Property(e => e.VehiclePlate).HasColumnName("VehiclePlate");
                builder.Property(e => e.Details).HasColumnName("Details");
                builder.Property(e => e.Status).HasColumnName("Status");
                builder.Property(e => e.CostCents).HasColumnName("CostCents");

                builder.Property(e => e.TaskDetailsChangedEvent_Details).HasColumnName("TaskDetailsChangedEvent_Details");
                builder.Property(e => e.TaskDetailsChangedEvent_TaskId).HasColumnName("TaskDetailsChangedEvent_TaskId");
                builder.Property(e => e.TaskId).HasColumnName("TaskId");
                builder.Property(e => e.TaskRemovedEvent_TaskId).HasColumnName("TaskRemovedEvent_TaskId");
                builder.Property(e => e.TaskStatusChangedEvent_Status).HasColumnName("TaskStatusChangedEvent_Status");
                builder.Property(e => e.TaskStatusChangedEvent_TaskId).HasColumnName("TaskStatusChangedEvent_TaskId");
            });

        }

        protected override void ConfigureConventions(ModelConfigurationBuilder configurationBuilder)
        {
            configurationBuilder
                .Properties<DateTimeOffset>()
                .HaveConversion<DateTimeOffsetConverter>();
            configurationBuilder.Properties<Enum>().HaveConversion<string>();
        }

    }


    //public class ApplicationDbContexttFactory : IDesignTimeDbContextFactory<ApplicationDbContext>
    //{
    //    public ApplicationDbContext CreateDbContext(string[] args)
    //    {
    //        var optionsBuilder = new DbContextOptionsBuilder<ApplicationDbContext>();
    //        optionsBuilder.UseSqlite("Data Source=default.db");

    //        return new ApplicationDbContext(optionsBuilder.Options);
    //    }
    //}
}


public class DateTimeOffsetConverter : ValueConverter<DateTimeOffset, DateTimeOffset>
{
    public DateTimeOffsetConverter()
        : base(
            d => d.ToUniversalTime(),
            d => d.ToUniversalTime())
    {
    }
}