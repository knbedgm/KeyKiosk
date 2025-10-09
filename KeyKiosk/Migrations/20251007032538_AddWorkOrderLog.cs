using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace KeyKiosk.Migrations
{
    /// <inheritdoc />
    public partial class AddWorkOrderLog : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_WorkOrderTasks_WorkOrders_WorkOrderId",
                table: "WorkOrderTasks");

            migrationBuilder.RenameColumn(
                name: "TaskDescription",
                table: "WorkOrderTaskTemplates",
                newName: "TaskTitle");

            migrationBuilder.RenameColumn(
                name: "Description",
                table: "WorkOrderTasks",
                newName: "Title");

            migrationBuilder.AddColumn<string>(
                name: "TaskDetails",
                table: "WorkOrderTaskTemplates",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AlterColumn<int>(
                name: "WorkOrderId",
                table: "WorkOrderTasks",
                type: "integer",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "integer",
                oldNullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Details",
                table: "WorkOrderTasks",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.CreateTable(
                name: "WorkOrderLog",
                columns: table => new
                {
                    ID = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    DateTime = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    UserId = table.Column<int>(type: "integer", nullable: false),
                    UserName = table.Column<string>(type: "text", nullable: false),
                    workOrderId = table.Column<int>(type: "integer", nullable: false),
                    EventType = table.Column<string>(type: "text", nullable: false),
                    CustomerName = table.Column<string>(type: "text", nullable: true),
                    VehiclePlate = table.Column<string>(type: "text", nullable: true),
                    Details = table.Column<string>(type: "text", nullable: true),
                    Status = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_WorkOrderLog", x => x.ID);
                    table.ForeignKey(
                        name: "FK_WorkOrderLog_WorkOrders_workOrderId",
                        column: x => x.workOrderId,
                        principalTable: "WorkOrders",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_WorkOrderLog_workOrderId",
                table: "WorkOrderLog",
                column: "workOrderId");

            migrationBuilder.AddForeignKey(
                name: "FK_WorkOrderTasks_WorkOrders_WorkOrderId",
                table: "WorkOrderTasks",
                column: "WorkOrderId",
                principalTable: "WorkOrders",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_WorkOrderTasks_WorkOrders_WorkOrderId",
                table: "WorkOrderTasks");

            migrationBuilder.DropTable(
                name: "WorkOrderLog");

            migrationBuilder.DropColumn(
                name: "TaskDetails",
                table: "WorkOrderTaskTemplates");

            migrationBuilder.DropColumn(
                name: "Details",
                table: "WorkOrderTasks");

            migrationBuilder.RenameColumn(
                name: "TaskTitle",
                table: "WorkOrderTaskTemplates",
                newName: "TaskDescription");

            migrationBuilder.RenameColumn(
                name: "Title",
                table: "WorkOrderTasks",
                newName: "Description");

            migrationBuilder.AlterColumn<int>(
                name: "WorkOrderId",
                table: "WorkOrderTasks",
                type: "integer",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "integer");

            migrationBuilder.AddForeignKey(
                name: "FK_WorkOrderTasks_WorkOrders_WorkOrderId",
                table: "WorkOrderTasks",
                column: "WorkOrderId",
                principalTable: "WorkOrders",
                principalColumn: "Id");
        }
    }
}
