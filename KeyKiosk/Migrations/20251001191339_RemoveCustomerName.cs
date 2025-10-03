using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace KeyKiosk.Migrations
{
    /// <inheritdoc />
    public partial class RemoveCustomerName : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CustomerName",
                table: "WorkOrderTasks");

            migrationBuilder.AddColumn<string>(
                name: "Description",
                table: "WorkOrderTasks",
                type: "text",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "CustomerName",
                table: "WorkOrderTasks",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.DropColumn(
                name: "Description",
                table: "WorkOrderTask");
        }
    }
}
