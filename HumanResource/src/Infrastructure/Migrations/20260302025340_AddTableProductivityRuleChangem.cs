using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddTableProductivityRuleChangem : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "BonusAmount",
                table: "productivity_rules");

            migrationBuilder.DropColumn(
                name: "IsPercentage",
                table: "productivity_rules");

            migrationBuilder.DropColumn(
                name: "IsMandatory",
                table: "deduction_rules");

            migrationBuilder.AddColumn<string>(
                name: "BonusType",
                table: "productivity_rules",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<decimal>(
                name: "BonusValue",
                table: "productivity_rules",
                type: "numeric(10,2)",
                precision: 10,
                scale: 2,
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "FullBonusTarget",
                table: "productivity_rules",
                type: "numeric(10,2)",
                precision: 10,
                scale: 2,
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "MaxBonusCap",
                table: "productivity_rules",
                type: "numeric",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Descripcion",
                table: "deduction_rules",
                type: "text",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "BonusType",
                table: "productivity_rules");

            migrationBuilder.DropColumn(
                name: "BonusValue",
                table: "productivity_rules");

            migrationBuilder.DropColumn(
                name: "FullBonusTarget",
                table: "productivity_rules");

            migrationBuilder.DropColumn(
                name: "MaxBonusCap",
                table: "productivity_rules");

            migrationBuilder.DropColumn(
                name: "Descripcion",
                table: "deduction_rules");

            migrationBuilder.AddColumn<decimal>(
                name: "BonusAmount",
                table: "productivity_rules",
                type: "numeric(18,2)",
                precision: 18,
                scale: 2,
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<bool>(
                name: "IsPercentage",
                table: "productivity_rules",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "IsMandatory",
                table: "deduction_rules",
                type: "boolean",
                nullable: false,
                defaultValue: false);
        }
    }
}
