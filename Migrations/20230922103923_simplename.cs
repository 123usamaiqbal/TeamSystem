using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TeamManageSystem.Migrations
{
    /// <inheritdoc />
    public partial class simplename : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "foldername",
                table: "ClickupFolders",
                newName: "name");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "name",
                table: "ClickupFolders",
                newName: "foldername");
        }
    }
}
