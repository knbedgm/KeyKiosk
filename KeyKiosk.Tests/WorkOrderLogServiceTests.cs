using KeyKiosk.Data;
using KeyKiosk.Services;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace KeyKiosk.Tests
{
    public class WorkOrderLogServiceTests
    {
        private ApplicationDbContext GetDbContextWithData()
        {
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .Options;

            var context = new ApplicationDbContext(options);

            // Create dummy users
            var dummyUser1 = new User { Id = 1, Name = "Tech1" };
            var dummyUser2 = new User { Id = 2, Name = "Tech2" };

            context.Users.AddRange(dummyUser1, dummyUser2);

            // Create a dummy work order
            var workOrder = new WorkOrder
            {
                Id = 1,
                CustomerName = "Alice",
                VehiclePlate = "ABC123",
                Details = "Oil change",
                Status = WorkOrderStatusType.Created,
                Tasks = new List<WorkOrderTask>(),
                Parts = new List<WorkOrderPart>()
            };

            context.WorkOrders.Add(workOrder);

            // Add dummy log events
            context.WorkOrderLog.AddRange(
                new WorkOrderLogEvent.CreateEvent
                {
                    ID = 1,
                    User = dummyUser1,   // ✅ sets UserId and UserName internally
                    workOrder = workOrder
                },
                new WorkOrderLogEvent.StatusChangedEvent
                {
                    ID = 2,
                    User = dummyUser2,   // ✅ sets UserId and UserName internally
                    workOrder = workOrder,
                    Status = WorkOrderStatusType.WorkStarted
                }
            );

            context.SaveChanges();
            return context;
        }


        [Fact]
        public async Task GetFilteredLogsAsync_ByUsername_ReturnsCorrectLog()
        {
            using var context = GetDbContextWithData();
            var service = new WorkOrderLogService(context);

            var results = await service.GetFilteredLogsAsync(username: "Tech1");

            Assert.Single(results);
            Assert.Equal("Tech1", results.First().UserName);
        }

        [Fact]
        public async Task ExportLogsToCsvAsync_ReturnsCsvWithDummyData()
        {
            using var context = GetDbContextWithData();
            var service = new WorkOrderLogService(context);

            var csv = await service.ExportLogsToCsvAsync();

            Assert.Contains("Tech1", csv);
            Assert.Contains("Tech2", csv);
            Assert.Contains("Oil change", csv);
        }
    }
}
