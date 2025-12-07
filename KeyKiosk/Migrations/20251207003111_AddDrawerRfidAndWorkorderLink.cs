using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace KeyKiosk.Migrations
{
    /// <inheritdoc />
    public partial class AddDrawerRfidAndWorkorderLink : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "CurrentRONumber",
                table: "Drawers",
                newName: "RFIDUid");

            migrationBuilder.RenameColumn(
                name: "RONumber",
                table: "DrawerLog",
                newName: "WorkorderNumber");

            migrationBuilder.AddColumn<int>(
                name: "CurrentWorkorderId",
                table: "Drawers",
                type: "integer",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Drawers_CurrentWorkorderId",
                table: "Drawers",
                column: "CurrentWorkorderId");

            migrationBuilder.AddForeignKey(
                name: "FK_Drawers_WorkOrders_CurrentWorkorderId",
                table: "Drawers",
                column: "CurrentWorkorderId",
                principalTable: "WorkOrders",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Drawers_WorkOrders_CurrentWorkorderId",
                table: "Drawers");

            migrationBuilder.DropIndex(
                name: "IX_Drawers_CurrentWorkorderId",
                table: "Drawers");

            migrationBuilder.DropColumn(
                name: "CurrentWorkorderId",
                table: "Drawers");

            migrationBuilder.RenameColumn(
                name: "RFIDUid",
                table: "Drawers",
                newName: "CurrentRONumber");

            migrationBuilder.RenameColumn(
                name: "WorkorderNumber",
                table: "DrawerLog",
                newName: "RONumber");
        }
    }
}
