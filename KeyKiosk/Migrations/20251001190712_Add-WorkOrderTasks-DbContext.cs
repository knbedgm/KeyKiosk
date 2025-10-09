using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace KeyKiosk.Migrations
{
    /// <inheritdoc />
    public partial class AddWorkOrderTasksDbContext : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_WorkOrderTask_WorkOrders_WorkOrderId",
                table: "WorkOrderTask");

            migrationBuilder.DropPrimaryKey(
                name: "PK_WorkOrderTask",
                table: "WorkOrderTask");

            migrationBuilder.RenameTable(
                name: "WorkOrderTask",
                newName: "WorkOrderTasks");

            migrationBuilder.RenameIndex(
                name: "IX_WorkOrderTask_WorkOrderId",
                table: "WorkOrderTasks",
                newName: "IX_WorkOrderTasks_WorkOrderId");

            migrationBuilder.AlterColumn<string>(
                name: "UserType",
                table: "Users",
                type: "text",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "TEXT");

            migrationBuilder.AlterColumn<string>(
                name: "EventType",
                table: "UserLog",
                type: "text",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "TEXT");

            migrationBuilder.AddPrimaryKey(
                name: "PK_WorkOrderTasks",
                table: "WorkOrderTasks",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_WorkOrderTasks_WorkOrders_WorkOrderId",
                table: "WorkOrderTasks",
                column: "WorkOrderId",
                principalTable: "WorkOrders",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_WorkOrderTasks_WorkOrders_WorkOrderId",
                table: "WorkOrderTasks");

            migrationBuilder.DropPrimaryKey(
                name: "PK_WorkOrderTasks",
                table: "WorkOrderTasks");

            migrationBuilder.RenameTable(
                name: "WorkOrderTasks",
                newName: "WorkOrderTask");

            migrationBuilder.RenameIndex(
                name: "IX_WorkOrderTasks_WorkOrderId",
                table: "WorkOrderTask",
                newName: "IX_WorkOrderTask_WorkOrderId");

            migrationBuilder.AlterColumn<string>(
                name: "UserType",
                table: "Users",
                type: "TEXT",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "text");

            migrationBuilder.AlterColumn<string>(
                name: "EventType",
                table: "UserLog",
                type: "TEXT",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "text");

            migrationBuilder.AddPrimaryKey(
                name: "PK_WorkOrderTask",
                table: "WorkOrderTask",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_WorkOrderTask_WorkOrders_WorkOrderId",
                table: "WorkOrderTask",
                column: "WorkOrderId",
                principalTable: "WorkOrders",
                principalColumn: "Id");
        }
    }
}
