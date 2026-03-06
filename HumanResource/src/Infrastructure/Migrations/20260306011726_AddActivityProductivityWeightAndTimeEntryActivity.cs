using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddActivityProductivityWeightAndTimeEntryActivity : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "ActivityName",
                table: "time_entries",
                type: "character varying(100)",
                maxLength: 100,
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "RedmineActivityId",
                table: "time_entries",
                type: "integer",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "activity_productivity_weights",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    RedmineActivityId = table.Column<int>(type: "integer", nullable: false),
                    ActivityName = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    Weight = table.Column<decimal>(type: "numeric(3,2)", precision: 3, scale: 2, nullable: false),
                    IsActive = table.Column<bool>(type: "boolean", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_activity_productivity_weights", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_activity_productivity_weights_RedmineActivityId",
                table: "activity_productivity_weights",
                column: "RedmineActivityId",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "activity_productivity_weights");

            migrationBuilder.DropColumn(
                name: "ActivityName",
                table: "time_entries");

            migrationBuilder.DropColumn(
                name: "RedmineActivityId",
                table: "time_entries");
        }
    }
}
