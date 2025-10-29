using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace KeyKiosk.Migrations
{
    /// <inheritdoc />
    public partial class DaysToHours : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "ExpectedDaysForCompletion",
                table: "WorkOrderTaskTemplates",
                newName: "ExpectedHoursForCompletion");

            migrationBuilder.RenameColumn(
                name: "DaysForCompletion",
                table: "WorkOrderTasks",
                newName: "HoursForCompletion");

            migrationBuilder.CreateIndex(
                name: "IX_WorkOrderLog_UserId",
                table: "WorkOrderLog",
                column: "UserId");

            migrationBuilder.AddForeignKey(
                name: "FK_WorkOrderLog_Users_UserId",
                table: "WorkOrderLog",
                column: "UserId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_WorkOrderLog_Users_UserId",
                table: "WorkOrderLog");

            migrationBuilder.DropIndex(
                name: "IX_WorkOrderLog_UserId",
                table: "WorkOrderLog");

            migrationBuilder.RenameColumn(
                name: "ExpectedHoursForCompletion",
                table: "WorkOrderTaskTemplates",
                newName: "ExpectedDaysForCompletion");

            migrationBuilder.RenameColumn(
                name: "HoursForCompletion",
                table: "WorkOrderTasks",
                newName: "DaysForCompletion");
        }
    }
}
