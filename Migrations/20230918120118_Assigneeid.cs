using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TeamManageSystem.Migrations
{
    /// <inheritdoc />
    public partial class Assigneeid : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "assignee_id",
                table: "ClickupAssignees",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "assignee_id",
                table: "ClickupAssignees");
        }
    }
}
