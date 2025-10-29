using KeyKiosk.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using static KeyKiosk.Data.WorkOrderLogEvent;

namespace KeyKiosk.Data
{
	// doesn't work, tries to apply to subclasses too
	//[EntityTypeConfiguration(typeof(WorkOrderLogEventEntityTypeConfiguration))]

	public abstract class WorkOrderLogEvent : ILogEvent
	{
        /// <summary>
        /// Base properties for all work order log events
        /// </summary>
        public int ID { get; set; }
		public DateTimeOffset DateTime { get; set; } = DateTimeOffset.Now;
		public int UserId { get; set; } 
        public string UserName { get; set; } = string.Empty;
        public User User { get; set; } = null!;
        //public required User User { set { UserId = value.Id; UserName = value.Name; } }
        public required WorkOrder workOrder { get; set; }
		public abstract WorkOrderLogEventType EventType { get; set; }

        /// <summary>
		/// Defines all possible event categories for work order log events
        /// </summary>
        public enum WorkOrderLogEventType
		{
			Created,
			StatusChanged,
			DetailsChanged,
			TaskAdded,
			TaskRemoved,
			TaskStatusChanged,
			TaskDetailsChanged,
		}

        /// <summary>
        /// SUBCLASSES
        /// </summary>
        public class CreateEvent : WorkOrderLogEvent
		{
			public override WorkOrderLogEventType EventType { get => WorkOrderLogEventType.Created; set {} }
		}

		public class StatusChangedEvent : WorkOrderLogEvent
		{
			public override WorkOrderLogEventType EventType { get => WorkOrderLogEventType.StatusChanged; set {} }
			public WorkOrderStatusType Status { get; set; }
		}


		public class DetailsChangedEvent : WorkOrderLogEvent
		{
			public override WorkOrderLogEventType EventType { get => WorkOrderLogEventType.DetailsChanged; set {} }

			public required string CustomerName { get; set; }
			public required string VehiclePlate { get; set; }
			public required string Details { get; set; }
		}

		//reference work order task for the changes
		public abstract class TaskEvent : WorkOrderLogEvent
		{
			public required WorkOrderTask Task { get; set; }
		}

		public class TaskAddedEvent : TaskEvent
		{
			public override WorkOrderLogEventType EventType { get => WorkOrderLogEventType.TaskAdded; set {} }
		}

		public class TaskRemovedEvent : TaskEvent
		{
			public override WorkOrderLogEventType EventType { get => WorkOrderLogEventType.TaskRemoved; set { } }
		}

		public class TaskStatusChangedEvent : TaskEvent
		{
			public override WorkOrderLogEventType EventType { get => WorkOrderLogEventType.TaskStatusChanged; set { } }

			public WorkOrderTaskStatusType Status { get; set; }
		}

		public class TaskDetailsChangedEvent : TaskEvent
		{
			public override WorkOrderLogEventType EventType { get => WorkOrderLogEventType.TaskDetailsChanged; set { } }

			public required string Details { get; set; } = "";
			public int CostCents { get; set; }
		}
	}

}

/// <summary>
/// Subclasses are mapped to CreateEvent, StatusChangedEventand DetailsChangedEvent
/// UseTphMappingStrategy(): Tells EF to use a single table for all subclasses.
/// HasDiscriminator(e => e.EventType): Uses the EventType property as the discriminator column.
/// HasValue<...>: Maps specific subclasses to specific enum values.
/// </summary>
public class WorkOrderLogEventEntityTypeConfiguration
    : IEntityTypeConfiguration<WorkOrderLogEvent>
{
    public void Configure(EntityTypeBuilder<WorkOrderLogEvent> builder)
    {
        builder.ToTable("WorkOrderLog");

        builder.HasDiscriminator<WorkOrderLogEvent.WorkOrderLogEventType>("EventType")
            .HasValue<WorkOrderLogEvent.CreateEvent>(WorkOrderLogEvent.WorkOrderLogEventType.Created)
            .HasValue<WorkOrderLogEvent.DetailsChangedEvent>(WorkOrderLogEvent.WorkOrderLogEventType.DetailsChanged)
            .HasValue<WorkOrderLogEvent.StatusChangedEvent>(WorkOrderLogEvent.WorkOrderLogEventType.StatusChanged)
            .HasValue<WorkOrderLogEvent.TaskAddedEvent>(WorkOrderLogEvent.WorkOrderLogEventType.TaskAdded)
            .HasValue<WorkOrderLogEvent.TaskRemovedEvent>(WorkOrderLogEvent.WorkOrderLogEventType.TaskRemoved)
            .HasValue<WorkOrderLogEvent.TaskDetailsChangedEvent>(WorkOrderLogEvent.WorkOrderLogEventType.TaskDetailsChanged)
            .HasValue<WorkOrderLogEvent.TaskStatusChangedEvent>(WorkOrderLogEvent.WorkOrderLogEventType.TaskStatusChanged);

        // Relationships
        builder.HasOne(e => e.workOrder)
               .WithMany()
               .HasForeignKey("workOrderId");

        builder.HasOne(e => e.User)
               .WithMany()
               .HasForeignKey("UserId");
    }
}
