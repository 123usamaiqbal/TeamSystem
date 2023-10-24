using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TeamManageSystem.Migrations
{
    /// <inheritdoc />
    public partial class Removetableandaddone : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ClickupAssignees");

            migrationBuilder.DropTable(
                name: "ClickupTasks");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "ClickupAssignees",
                columns: table => new
                {
                    id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    assigneeid = table.Column<int>(type: "int", nullable: false),
                    taskid = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    username = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ClickupAssignees", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "ClickupTasks",
                columns: table => new
                {
                    id = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    color = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    date_closed = table.Column<DateTime>(type: "datetime2", nullable: true),
                    date_created = table.Column<DateTime>(type: "datetime2", nullable: true),
                    date_done = table.Column<DateTime>(type: "datetime2", nullable: true),
                    date_updated = table.Column<DateTime>(type: "datetime2", nullable: true),
                    description = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    list_id = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    orderindex = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    statusvalue = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    type = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ClickupTasks", x => x.id);
                });
        }
    }
}
