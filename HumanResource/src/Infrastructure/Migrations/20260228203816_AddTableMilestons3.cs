using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddTableMilestons3 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsPaid",
                table: "project_milestones");

            migrationBuilder.AddColumn<string>(
                name: "Status",
                table: "project_milestones",
                type: "text",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Status",
                table: "project_milestones");

            migrationBuilder.AddColumn<bool>(
                name: "IsPaid",
                table: "project_milestones",
                type: "boolean",
                nullable: false,
                defaultValue: false);
        }
    }
}
