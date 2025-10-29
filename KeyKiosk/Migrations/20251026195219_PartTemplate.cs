using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace KeyKiosk.Migrations
{
    /// <inheritdoc />
    public partial class PartTemplate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_WorkOrderPart_WorkOrders_WorkOrderId",
                table: "WorkOrderPart");

            migrationBuilder.DropPrimaryKey(
                name: "PK_WorkOrderPart",
                table: "WorkOrderPart");

            migrationBuilder.RenameTable(
                name: "WorkOrderPart",
                newName: "WorkOrderParts");

            migrationBuilder.RenameIndex(
                name: "IX_WorkOrderPart_WorkOrderId",
                table: "WorkOrderParts",
                newName: "IX_WorkOrderParts_WorkOrderId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_WorkOrderParts",
                table: "WorkOrderParts",
                column: "Id");

            migrationBuilder.CreateTable(
                name: "PartTemplates",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    PartName = table.Column<string>(type: "text", nullable: false),
                    Details = table.Column<string>(type: "text", nullable: false),
                    CostCents = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PartTemplates", x => x.Id);
                });

            migrationBuilder.AddForeignKey(
                name: "FK_WorkOrderParts_WorkOrders_WorkOrderId",
                table: "WorkOrderParts",
                column: "WorkOrderId",
                principalTable: "WorkOrders",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_WorkOrderParts_WorkOrders_WorkOrderId",
                table: "WorkOrderParts");

            migrationBuilder.DropTable(
                name: "PartTemplates");

            migrationBuilder.DropPrimaryKey(
                name: "PK_WorkOrderParts",
                table: "WorkOrderParts");

            migrationBuilder.RenameTable(
                name: "WorkOrderParts",
                newName: "WorkOrderPart");

            migrationBuilder.RenameIndex(
                name: "IX_WorkOrderParts_WorkOrderId",
                table: "WorkOrderPart",
                newName: "IX_WorkOrderPart_WorkOrderId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_WorkOrderPart",
                table: "WorkOrderPart",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_WorkOrderPart_WorkOrders_WorkOrderId",
                table: "WorkOrderPart",
                column: "WorkOrderId",
                principalTable: "WorkOrders",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
