﻿using Microsoft.EntityFrameworkCore;
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

        //protected override void OnModelCreating(ModelBuilder modelBuilder)
        //{
        //    modelBuilder.Entity<User>().Property(e => e.UserType).HasConversion<string>();
        //    modelBuilder.Entity<UserLogEvent>().Property(e => e.EventType).HasConversion<string>();
        //    modelBuilder.Entity<DrawerLogEvent>().Property(e => e.EventType).HasConversion<string>();
        //}

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