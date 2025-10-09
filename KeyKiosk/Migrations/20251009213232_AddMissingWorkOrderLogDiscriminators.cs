using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace KeyKiosk.Migrations
{
    /// <inheritdoc />
    public partial class AddMissingWorkOrderLogDiscriminators : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "CostCents",
                table: "WorkOrderLog",
                type: "integer",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "TaskDetailsChangedEvent_Details",
                table: "WorkOrderLog",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "TaskDetailsChangedEvent_TaskId",
                table: "WorkOrderLog",
                type: "integer",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "TaskId",
                table: "WorkOrderLog",
                type: "integer",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "TaskRemovedEvent_TaskId",
                table: "WorkOrderLog",
                type: "integer",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "TaskStatusChangedEvent_Status",
                table: "WorkOrderLog",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "TaskStatusChangedEvent_TaskId",
                table: "WorkOrderLog",
                type: "integer",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_WorkOrderLog_TaskDetailsChangedEvent_TaskId",
                table: "WorkOrderLog",
                column: "TaskDetailsChangedEvent_TaskId");

            migrationBuilder.CreateIndex(
                name: "IX_WorkOrderLog_TaskId",
                table: "WorkOrderLog",
                column: "TaskId");

            migrationBuilder.CreateIndex(
                name: "IX_WorkOrderLog_TaskRemovedEvent_TaskId",
                table: "WorkOrderLog",
                column: "TaskRemovedEvent_TaskId");

            migrationBuilder.CreateIndex(
                name: "IX_WorkOrderLog_TaskStatusChangedEvent_TaskId",
                table: "WorkOrderLog",
                column: "TaskStatusChangedEvent_TaskId");

            migrationBuilder.AddForeignKey(
                name: "FK_WorkOrderLog_WorkOrderTasks_TaskDetailsChangedEvent_TaskId",
                table: "WorkOrderLog",
                column: "TaskDetailsChangedEvent_TaskId",
                principalTable: "WorkOrderTasks",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_WorkOrderLog_WorkOrderTasks_TaskId",
                table: "WorkOrderLog",
                column: "TaskId",
                principalTable: "WorkOrderTasks",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_WorkOrderLog_WorkOrderTasks_TaskRemovedEvent_TaskId",
                table: "WorkOrderLog",
                column: "TaskRemovedEvent_TaskId",
                principalTable: "WorkOrderTasks",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_WorkOrderLog_WorkOrderTasks_TaskStatusChangedEvent_TaskId",
                table: "WorkOrderLog",
                column: "TaskStatusChangedEvent_TaskId",
                principalTable: "WorkOrderTasks",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_WorkOrderLog_WorkOrderTasks_TaskDetailsChangedEvent_TaskId",
                table: "WorkOrderLog");

            migrationBuilder.DropForeignKey(
                name: "FK_WorkOrderLog_WorkOrderTasks_TaskId",
                table: "WorkOrderLog");

            migrationBuilder.DropForeignKey(
                name: "FK_WorkOrderLog_WorkOrderTasks_TaskRemovedEvent_TaskId",
                table: "WorkOrderLog");

            migrationBuilder.DropForeignKey(
                name: "FK_WorkOrderLog_WorkOrderTasks_TaskStatusChangedEvent_TaskId",
                table: "WorkOrderLog");

            migrationBuilder.DropIndex(
                name: "IX_WorkOrderLog_TaskDetailsChangedEvent_TaskId",
                table: "WorkOrderLog");

            migrationBuilder.DropIndex(
                name: "IX_WorkOrderLog_TaskId",
                table: "WorkOrderLog");

            migrationBuilder.DropIndex(
                name: "IX_WorkOrderLog_TaskRemovedEvent_TaskId",
                table: "WorkOrderLog");

            migrationBuilder.DropIndex(
                name: "IX_WorkOrderLog_TaskStatusChangedEvent_TaskId",
                table: "WorkOrderLog");

            migrationBuilder.DropColumn(
                name: "CostCents",
                table: "WorkOrderLog");

            migrationBuilder.DropColumn(
                name: "TaskDetailsChangedEvent_Details",
                table: "WorkOrderLog");

            migrationBuilder.DropColumn(
                name: "TaskDetailsChangedEvent_TaskId",
                table: "WorkOrderLog");

            migrationBuilder.DropColumn(
                name: "TaskId",
                table: "WorkOrderLog");

            migrationBuilder.DropColumn(
                name: "TaskRemovedEvent_TaskId",
                table: "WorkOrderLog");

            migrationBuilder.DropColumn(
                name: "TaskStatusChangedEvent_Status",
                table: "WorkOrderLog");

            migrationBuilder.DropColumn(
                name: "TaskStatusChangedEvent_TaskId",
                table: "WorkOrderLog");
        }
    }
}
