using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TeamManageSystem.Migrations
{
    /// <inheritdoc />
    public partial class Addassigneeid : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "statusid",
                table: "ClickupTasks",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "ClickupTasksid",
                table: "ClickupAssignees",
                type: "nvarchar(450)",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "assigneeid",
                table: "ClickupAssignees",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateTable(
                name: "StatusInfo",
                columns: table => new
                {
                    id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    status = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    color = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    type = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    orderindex = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_StatusInfo", x => x.id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ClickupTasks_statusid",
                table: "ClickupTasks",
                column: "statusid");

            migrationBuilder.CreateIndex(
                name: "IX_ClickupAssignees_ClickupTasksid",
                table: "ClickupAssignees",
                column: "ClickupTasksid");

            migrationBuilder.AddForeignKey(
                name: "FK_ClickupAssignees_ClickupTasks_ClickupTasksid",
                table: "ClickupAssignees",
                column: "ClickupTasksid",
                principalTable: "ClickupTasks",
                principalColumn: "id");

            migrationBuilder.AddForeignKey(
                name: "FK_ClickupTasks_StatusInfo_statusid",
                table: "ClickupTasks",
                column: "statusid",
                principalTable: "StatusInfo",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ClickupAssignees_ClickupTasks_ClickupTasksid",
                table: "ClickupAssignees");

            migrationBuilder.DropForeignKey(
                name: "FK_ClickupTasks_StatusInfo_statusid",
                table: "ClickupTasks");

            migrationBuilder.DropTable(
                name: "StatusInfo");

            migrationBuilder.DropIndex(
                name: "IX_ClickupTasks_statusid",
                table: "ClickupTasks");

            migrationBuilder.DropIndex(
                name: "IX_ClickupAssignees_ClickupTasksid",
                table: "ClickupAssignees");

            migrationBuilder.DropColumn(
                name: "statusid",
                table: "ClickupTasks");

            migrationBuilder.DropColumn(
                name: "ClickupTasksid",
                table: "ClickupAssignees");

            migrationBuilder.DropColumn(
                name: "assigneeid",
                table: "ClickupAssignees");
        }
    }
}
