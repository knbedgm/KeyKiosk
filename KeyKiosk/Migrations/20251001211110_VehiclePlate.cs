using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace KeyKiosk.Migrations
{
    /// <inheritdoc />
    public partial class VehiclePlate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "VehiclePlate",
                table: "WorkOrders",
                type: "text",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "VehiclePlate",
                table: "WorkOrders");
        }
    }
}
