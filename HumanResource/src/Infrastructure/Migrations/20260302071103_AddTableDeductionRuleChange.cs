using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddTableDeductionRuleChange : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Descripcion",
                table: "deduction_rules",
                newName: "Type");

            migrationBuilder.AddColumn<Guid>(
                name: "RuleId",
                table: "payroll_components",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddColumn<string>(
                name: "Description",
                table: "deduction_rules",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<DateTime>(
                name: "CreatedAt",
                table: "aguinaldo_payments",
                type: "timestamp with time zone",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<DateTime>(
                name: "UpdatedAt",
                table: "aguinaldo_payments",
                type: "timestamp with time zone",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "RuleId",
                table: "payroll_components");

            migrationBuilder.DropColumn(
                name: "Description",
                table: "deduction_rules");

            migrationBuilder.DropColumn(
                name: "CreatedAt",
                table: "aguinaldo_payments");

            migrationBuilder.DropColumn(
                name: "UpdatedAt",
                table: "aguinaldo_payments");

            migrationBuilder.RenameColumn(
                name: "Type",
                table: "deduction_rules",
                newName: "Descripcion");
        }
    }
}
