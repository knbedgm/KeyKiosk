using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace KeyKiosk.Migrations
{
    /// <inheritdoc />
    public partial class ChangeLogTypes : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_DrawerLog_Drawers_DrawerId",
                table: "DrawerLog");

            migrationBuilder.DropForeignKey(
                name: "FK_DrawerLog_Users_UserId",
                table: "DrawerLog");

            migrationBuilder.DropForeignKey(
                name: "FK_UserLog_Users_ActingUserId",
                table: "UserLog");

            migrationBuilder.DropIndex(
                name: "IX_UserLog_ActingUserId",
                table: "UserLog");

            migrationBuilder.DropIndex(
                name: "IX_DrawerLog_DrawerId",
                table: "DrawerLog");

            migrationBuilder.DropIndex(
                name: "IX_DrawerLog_UserId",
                table: "DrawerLog");

            migrationBuilder.AddColumn<string>(
                name: "ActingUserName",
                table: "UserLog",
                type: "TEXT",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<int>(
                name: "SecondaryUserId",
                table: "UserLog",
                type: "INTEGER",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "UserName",
                table: "DrawerLog",
                type: "TEXT",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ActingUserName",
                table: "UserLog");

            migrationBuilder.DropColumn(
                name: "SecondaryUserId",
                table: "UserLog");

            migrationBuilder.DropColumn(
                name: "UserName",
                table: "DrawerLog");

            migrationBuilder.CreateIndex(
                name: "IX_UserLog_ActingUserId",
                table: "UserLog",
                column: "ActingUserId");

            migrationBuilder.CreateIndex(
                name: "IX_DrawerLog_DrawerId",
                table: "DrawerLog",
                column: "DrawerId");

            migrationBuilder.CreateIndex(
                name: "IX_DrawerLog_UserId",
                table: "DrawerLog",
                column: "UserId");

            migrationBuilder.AddForeignKey(
                name: "FK_DrawerLog_Drawers_DrawerId",
                table: "DrawerLog",
                column: "DrawerId",
                principalTable: "Drawers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_DrawerLog_Users_UserId",
                table: "DrawerLog",
                column: "UserId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_UserLog_Users_ActingUserId",
                table: "UserLog",
                column: "ActingUserId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
