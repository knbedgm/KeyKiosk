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
		public int ID { get; set; }
		public DateTimeOffset DateTime { get; set; } = DateTimeOffset.Now;
		public int UserId { get; private set; }
		public string UserName { get; private set; }
		public required User User { set { UserId = value.Id; UserName = value.Name; } }
		public required WorkOrder workOrder { get; set; }
		public abstract WorkOrderLogEventType EventType { get; set; }

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

public class WorkOrderLogEventEntityTypeConfiguration : IEntityTypeConfiguration<WorkOrderLogEvent>
{
	public void Configure(EntityTypeBuilder<WorkOrderLogEvent> builder)
	{
		builder
			.UseTphMappingStrategy()
			.HasDiscriminator(e => e.EventType)
			.HasValue<CreateEvent>(WorkOrderLogEventType.Created)
			.HasValue<StatusChangedEvent>(WorkOrderLogEventType.StatusChanged)
			.HasValue<DetailsChangedEvent>(WorkOrderLogEventType.DetailsChanged)
			.HasValue<TaskAddedEvent>(WorkOrderLogEventType.TaskAdded)
			.HasValue<TaskRemovedEvent>(WorkOrderLogEventType.TaskRemoved)
			.HasValue<TaskStatusChangedEvent>(WorkOrderLogEventType.TaskStatusChanged)
			.HasValue<TaskDetailsChangedEvent>(WorkOrderLogEventType.TaskDetailsChanged)
			;
	}
}