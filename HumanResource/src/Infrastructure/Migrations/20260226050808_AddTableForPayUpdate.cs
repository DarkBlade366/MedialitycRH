using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddTableForPayUpdate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_payrolls_employees_EmployeeId",
                table: "payrolls");

            migrationBuilder.DropTable(
                name: "role_salaries");

            migrationBuilder.DropIndex(
                name: "IX_payrolls_EmployeeId_From_To",
                table: "payrolls");

            migrationBuilder.DropColumn(
                name: "From",
                table: "payrolls");

            migrationBuilder.RenameColumn(
                name: "To",
                table: "payrolls",
                newName: "PeriodTo");

            migrationBuilder.RenameColumn(
                name: "GeneratedAt",
                table: "payrolls",
                newName: "PeriodFrom");

            migrationBuilder.RenameColumn(
                name: "ProjectId",
                table: "payroll_lines",
                newName: "RedmineProjectId");

            migrationBuilder.AlterColumn<decimal>(
                name: "TotalHours",
                table: "payrolls",
                type: "numeric(12,2)",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "numeric(10,2)");

            migrationBuilder.AlterColumn<decimal>(
                name: "TotalAmount",
                table: "payrolls",
                type: "numeric(14,2)",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "numeric(10,2)");

            migrationBuilder.AddColumn<string>(
                name: "Status",
                table: "payrolls",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.CreateTable(
                name: "payroll_components",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    PayrollId = table.Column<Guid>(type: "uuid", nullable: false),
                    Type = table.Column<string>(type: "text", nullable: false),
                    Description = table.Column<string>(type: "character varying(300)", maxLength: 300, nullable: false),
                    Amount = table.Column<decimal>(type: "numeric(14,2)", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_payroll_components", x => x.Id);
                    table.ForeignKey(
                        name: "FK_payroll_components_payrolls_PayrollId",
                        column: x => x.PayrollId,
                        principalTable: "payrolls",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "project_bonus_configurations",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    RedmineProjectId = table.Column<int>(type: "integer", nullable: false),
                    ExtraHourlyRate = table.Column<decimal>(type: "numeric(10,2)", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_project_bonus_configurations", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "salary_configurations",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Role = table.Column<string>(type: "text", nullable: false),
                    BaseHourlyRate = table.Column<decimal>(type: "numeric(10,2)", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_salary_configurations", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_payrolls_EmployeeId_PeriodFrom_PeriodTo",
                table: "payrolls",
                columns: new[] { "EmployeeId", "PeriodFrom", "PeriodTo" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_payroll_components_PayrollId",
                table: "payroll_components",
                column: "PayrollId");

            migrationBuilder.CreateIndex(
                name: "IX_project_bonus_configurations_RedmineProjectId",
                table: "project_bonus_configurations",
                column: "RedmineProjectId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_salary_configurations_Role",
                table: "salary_configurations",
                column: "Role",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "payroll_components");

            migrationBuilder.DropTable(
                name: "project_bonus_configurations");

            migrationBuilder.DropTable(
                name: "salary_configurations");

            migrationBuilder.DropIndex(
                name: "IX_payrolls_EmployeeId_PeriodFrom_PeriodTo",
                table: "payrolls");

            migrationBuilder.DropColumn(
                name: "Status",
                table: "payrolls");

            migrationBuilder.RenameColumn(
                name: "PeriodTo",
                table: "payrolls",
                newName: "To");

            migrationBuilder.RenameColumn(
                name: "PeriodFrom",
                table: "payrolls",
                newName: "GeneratedAt");

            migrationBuilder.RenameColumn(
                name: "RedmineProjectId",
                table: "payroll_lines",
                newName: "ProjectId");

            migrationBuilder.AlterColumn<decimal>(
                name: "TotalHours",
                table: "payrolls",
                type: "numeric(10,2)",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "numeric(12,2)");

            migrationBuilder.AlterColumn<decimal>(
                name: "TotalAmount",
                table: "payrolls",
                type: "numeric(10,2)",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "numeric(14,2)");

            migrationBuilder.AddColumn<DateTime>(
                name: "From",
                table: "payrolls",
                type: "timestamp with time zone",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.CreateTable(
                name: "role_salaries",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    BaseHourlyRate = table.Column<decimal>(type: "numeric(10,2)", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    Role = table.Column<string>(type: "text", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_role_salaries", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_payrolls_EmployeeId_From_To",
                table: "payrolls",
                columns: new[] { "EmployeeId", "From", "To" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_role_salaries_Role",
                table: "role_salaries",
                column: "Role",
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_payrolls_employees_EmployeeId",
                table: "payrolls",
                column: "EmployeeId",
                principalTable: "employees",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
