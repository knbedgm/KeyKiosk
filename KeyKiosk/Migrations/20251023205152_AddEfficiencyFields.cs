using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace KeyKiosk.Migrations
{
    /// <inheritdoc />
    public partial class AddEfficiencyFields : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "ExpectedDaysForCompletion",
                table: "WorkOrderTaskTemplates",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "DaysForCompletion",
                table: "WorkOrderTasks",
                type: "integer",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ExpectedDaysForCompletion",
                table: "WorkOrderTaskTemplates");

            migrationBuilder.DropColumn(
                name: "DaysForCompletion",
                table: "WorkOrderTasks");
        }
    }
}
