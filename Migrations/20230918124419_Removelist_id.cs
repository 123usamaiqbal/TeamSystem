using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TeamManageSystem.Migrations
{
    /// <inheritdoc />
    public partial class Removelist_id : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "list_id",
                table: "ClickupLists");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "list_id",
                table: "ClickupLists",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }
    }
}
