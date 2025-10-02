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
                table: "WorkOrdersTask");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "CustomerName",
                table: "WorkOrdersTask",
                type: "text",
                nullable: false,
                defaultValue: "");
        }
    }
}
