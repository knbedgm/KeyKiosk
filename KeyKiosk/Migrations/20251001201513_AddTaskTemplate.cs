using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace KeyKiosk.Migrations
{
    /// <inheritdoc />
    public partial class AddTaskTemplate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_WorkOrdersTask_WorkOrders_WorkOrderId",
                table: "WorkOrdersTask");

            migrationBuilder.DropPrimaryKey(
                name: "PK_WorkOrdersTask",
                table: "WorkOrdersTask");

            migrationBuilder.RenameTable(
                name: "WorkOrdersTask",
                newName: "WorkOrderTask");

            migrationBuilder.RenameIndex(
                name: "IX_WorkOrdersTask_WorkOrderId",
                table: "WorkOrderTask",
                newName: "IX_WorkOrderTask_WorkOrderId");

            migrationBuilder.AddColumn<string>(
                name: "Description",
                table: "WorkOrderTask",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddPrimaryKey(
                name: "PK_WorkOrderTask",
                table: "WorkOrderTask",
                column: "Id");

            migrationBuilder.CreateTable(
                name: "WorkOrderTaskTemplates",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    TaskDescription = table.Column<string>(type: "text", nullable: false),
                    TaskCostCents = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_WorkOrderTaskTemplates", x => x.Id);
                });

            migrationBuilder.AddForeignKey(
                name: "FK_WorkOrderTask_WorkOrders_WorkOrderId",
                table: "WorkOrderTask",
                column: "WorkOrderId",
                principalTable: "WorkOrders",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_WorkOrderTask_WorkOrders_WorkOrderId",
                table: "WorkOrderTask");

            migrationBuilder.DropTable(
                name: "WorkOrderTaskTemplates");

            migrationBuilder.DropPrimaryKey(
                name: "PK_WorkOrderTask",
                table: "WorkOrderTask");

            migrationBuilder.DropColumn(
                name: "Description",
                table: "WorkOrderTask");

            migrationBuilder.RenameTable(
                name: "WorkOrderTask",
                newName: "WorkOrdersTask");

            migrationBuilder.RenameIndex(
                name: "IX_WorkOrderTask_WorkOrderId",
                table: "WorkOrdersTask",
                newName: "IX_WorkOrdersTask_WorkOrderId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_WorkOrdersTask",
                table: "WorkOrdersTask",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_WorkOrdersTask_WorkOrders_WorkOrderId",
                table: "WorkOrdersTask",
                column: "WorkOrderId",
                principalTable: "WorkOrders",
                principalColumn: "Id");
        }
    }
}
