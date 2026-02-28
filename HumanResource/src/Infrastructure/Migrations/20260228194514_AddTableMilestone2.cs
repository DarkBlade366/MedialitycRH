using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddTableMilestone2 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_ProjectMilestones",
                table: "ProjectMilestones");

            migrationBuilder.RenameTable(
                name: "ProjectMilestones",
                newName: "project_milestones");

            migrationBuilder.AlterColumn<string>(
                name: "Name",
                table: "project_milestones",
                type: "character varying(200)",
                maxLength: 200,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "text");

            migrationBuilder.AddPrimaryKey(
                name: "PK_project_milestones",
                table: "project_milestones",
                column: "Id");

            migrationBuilder.CreateIndex(
                name: "IX_project_milestones_RedmineProjectId_Name",
                table: "project_milestones",
                columns: new[] { "RedmineProjectId", "Name" },
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_project_milestones",
                table: "project_milestones");

            migrationBuilder.DropIndex(
                name: "IX_project_milestones_RedmineProjectId_Name",
                table: "project_milestones");

            migrationBuilder.RenameTable(
                name: "project_milestones",
                newName: "ProjectMilestones");

            migrationBuilder.AlterColumn<string>(
                name: "Name",
                table: "ProjectMilestones",
                type: "text",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "character varying(200)",
                oldMaxLength: 200);

            migrationBuilder.AddPrimaryKey(
                name: "PK_ProjectMilestones",
                table: "ProjectMilestones",
                column: "Id");
        }
    }
}
