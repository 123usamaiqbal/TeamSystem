using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TeamManageSystem.Migrations
{
    /// <inheritdoc />
    public partial class StatusColors : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "StatusColors",
                columns: table => new
                {
                    id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    statusvalue = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    color = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_StatusColors", x => x.id);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "StatusColors");
        }
    }
}
