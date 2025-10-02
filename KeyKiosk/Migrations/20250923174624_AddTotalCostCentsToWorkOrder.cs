using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace KeyKiosk.Migrations
{
    /// <inheritdoc />
    public partial class AddTotalCostCentsToWorkOrder : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_WorkOrdersTask_WorkOrders_WorkOrderId",
                table: "WorkOrdersTask");

            migrationBuilder.DropColumn(
                name: "EndDate",
                table: "WorkOrdersTask");

            migrationBuilder.DropColumn(
                name: "StartDate",
                table: "WorkOrdersTask");

            migrationBuilder.RenameColumn(
                name: "CustomerName",
                table: "WorkOrdersTask",
                newName: "Description");

            migrationBuilder.AlterColumn<int>(
                name: "WorkOrderId",
                table: "WorkOrdersTask",
                type: "integer",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "integer",
                oldNullable: true);

            migrationBuilder.AddColumn<int>(
                name: "TotalCostCents",
                table: "WorkOrders",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddForeignKey(
                name: "FK_WorkOrdersTask_WorkOrders_WorkOrderId",
                table: "WorkOrdersTask",
                column: "WorkOrderId",
                principalTable: "WorkOrders",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_WorkOrdersTask_WorkOrders_WorkOrderId",
                table: "WorkOrdersTask");

            migrationBuilder.DropColumn(
                name: "TotalCostCents",
                table: "WorkOrders");

            migrationBuilder.RenameColumn(
                name: "Description",
                table: "WorkOrdersTask",
                newName: "CustomerName");

            migrationBuilder.AlterColumn<int>(
                name: "WorkOrderId",
                table: "WorkOrdersTask",
                type: "integer",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "integer");

            migrationBuilder.AddColumn<DateTimeOffset>(
                name: "EndDate",
                table: "WorkOrdersTask",
                type: "timestamp with time zone",
                nullable: false,
                defaultValue: new DateTimeOffset(new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)));

            migrationBuilder.AddColumn<DateTimeOffset>(
                name: "StartDate",
                table: "WorkOrdersTask",
                type: "timestamp with time zone",
                nullable: false,
                defaultValue: new DateTimeOffset(new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)));

            migrationBuilder.AddForeignKey(
                name: "FK_WorkOrdersTask_WorkOrders_WorkOrderId",
                table: "WorkOrdersTask",
                column: "WorkOrderId",
                principalTable: "WorkOrders",
                principalColumn: "Id");
        }
    }
}
