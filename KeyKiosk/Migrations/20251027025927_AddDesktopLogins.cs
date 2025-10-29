using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace KeyKiosk.Migrations
{
    /// <inheritdoc />
    public partial class AddDesktopLogins : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "Pin",
                table: "Users",
                type: "text",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "text");

            migrationBuilder.AddColumn<string>(
                name: "DesktopLogin_HashedPassword",
                table: "Users",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "DesktopLogin_Username",
                table: "Users",
                type: "text",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Users_DesktopLogin_Username",
                table: "Users",
                column: "DesktopLogin_Username",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Users_Pin",
                table: "Users",
                column: "Pin",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Users_DesktopLogin_Username",
                table: "Users");

            migrationBuilder.DropIndex(
                name: "IX_Users_Pin",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "DesktopLogin_HashedPassword",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "DesktopLogin_Username",
                table: "Users");

            migrationBuilder.AlterColumn<string>(
                name: "Pin",
                table: "Users",
                type: "text",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "text",
                oldNullable: true);
        }
    }
}
