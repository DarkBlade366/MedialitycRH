using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddTableAudi : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "CreatedBy",
                table: "vacation_rules",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "UpdatedBy",
                table: "vacation_rules",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "CreatedBy",
                table: "vacation_payments",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "UpdatedBy",
                table: "vacation_payments",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "CreatedBy",
                table: "time_entries",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "UpdatedBy",
                table: "time_entries",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "CreatedBy",
                table: "project_rules",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "UpdatedBy",
                table: "project_rules",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "CreatedBy",
                table: "project_milestones",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "UpdatedBy",
                table: "project_milestones",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "CreatedBy",
                table: "productivity_rules",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "UpdatedBy",
                table: "productivity_rules",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "CreatedBy",
                table: "productivity_payments",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "UpdatedBy",
                table: "productivity_payments",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "CreatedBy",
                table: "payrolls",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "UpdatedBy",
                table: "payrolls",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "CreatedBy",
                table: "payroll_components",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "UpdatedBy",
                table: "payroll_components",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "CreatedBy",
                table: "overtime_rules",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "UpdatedBy",
                table: "overtime_rules",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "CreatedBy",
                table: "overtime_payments",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "UpdatedBy",
                table: "overtime_payments",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "CreatedBy",
                table: "milestone_rules",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "UpdatedBy",
                table: "milestone_rules",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "CreatedBy",
                table: "milestone_payments",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "UpdatedBy",
                table: "milestone_payments",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "CreatedBy",
                table: "milestone_participations",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "UpdatedBy",
                table: "milestone_participations",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "CreatedBy",
                table: "employees",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "UpdatedBy",
                table: "employees",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "CreatedBy",
                table: "employee_vacation_balances",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "UpdatedBy",
                table: "employee_vacation_balances",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "CreatedBy",
                table: "employee_aguinaldo_balances",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "UpdatedBy",
                table: "employee_aguinaldo_balances",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "CreatedBy",
                table: "deduction_rules",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "UpdatedBy",
                table: "deduction_rules",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "CreatedBy",
                table: "deduction_payments",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "UpdatedBy",
                table: "deduction_payments",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "CreatedBy",
                table: "base_salary_rules",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "UpdatedBy",
                table: "base_salary_rules",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "CreatedBy",
                table: "aguinaldo_rules",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "UpdatedBy",
                table: "aguinaldo_rules",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "CreatedBy",
                table: "aguinaldo_payments",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "UpdatedBy",
                table: "aguinaldo_payments",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "CreatedBy",
                table: "activity_productivity_weights",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "UpdatedBy",
                table: "activity_productivity_weights",
                type: "text",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "audit_logs",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    EntityName = table.Column<string>(type: "text", nullable: false),
                    EntityId = table.Column<string>(type: "text", nullable: false),
                    Action = table.Column<string>(type: "text", nullable: false),
                    OldValues = table.Column<string>(type: "jsonb", nullable: true),
                    NewValues = table.Column<string>(type: "jsonb", nullable: true),
                    UserName = table.Column<string>(type: "text", nullable: false),
                    Timestamp = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_audit_logs", x => x.Id);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "audit_logs");

            migrationBuilder.DropColumn(
                name: "CreatedBy",
                table: "vacation_rules");

            migrationBuilder.DropColumn(
                name: "UpdatedBy",
                table: "vacation_rules");

            migrationBuilder.DropColumn(
                name: "CreatedBy",
                table: "vacation_payments");

            migrationBuilder.DropColumn(
                name: "UpdatedBy",
                table: "vacation_payments");

            migrationBuilder.DropColumn(
                name: "CreatedBy",
                table: "time_entries");

            migrationBuilder.DropColumn(
                name: "UpdatedBy",
                table: "time_entries");

            migrationBuilder.DropColumn(
                name: "CreatedBy",
                table: "project_rules");

            migrationBuilder.DropColumn(
                name: "UpdatedBy",
                table: "project_rules");

            migrationBuilder.DropColumn(
                name: "CreatedBy",
                table: "project_milestones");

            migrationBuilder.DropColumn(
                name: "UpdatedBy",
                table: "project_milestones");

            migrationBuilder.DropColumn(
                name: "CreatedBy",
                table: "productivity_rules");

            migrationBuilder.DropColumn(
                name: "UpdatedBy",
                table: "productivity_rules");

            migrationBuilder.DropColumn(
                name: "CreatedBy",
                table: "productivity_payments");

            migrationBuilder.DropColumn(
                name: "UpdatedBy",
                table: "productivity_payments");

            migrationBuilder.DropColumn(
                name: "CreatedBy",
                table: "payrolls");

            migrationBuilder.DropColumn(
                name: "UpdatedBy",
                table: "payrolls");

            migrationBuilder.DropColumn(
                name: "CreatedBy",
                table: "payroll_components");

            migrationBuilder.DropColumn(
                name: "UpdatedBy",
                table: "payroll_components");

            migrationBuilder.DropColumn(
                name: "CreatedBy",
                table: "overtime_rules");

            migrationBuilder.DropColumn(
                name: "UpdatedBy",
                table: "overtime_rules");

            migrationBuilder.DropColumn(
                name: "CreatedBy",
                table: "overtime_payments");

            migrationBuilder.DropColumn(
                name: "UpdatedBy",
                table: "overtime_payments");

            migrationBuilder.DropColumn(
                name: "CreatedBy",
                table: "milestone_rules");

            migrationBuilder.DropColumn(
                name: "UpdatedBy",
                table: "milestone_rules");

            migrationBuilder.DropColumn(
                name: "CreatedBy",
                table: "milestone_payments");

            migrationBuilder.DropColumn(
                name: "UpdatedBy",
                table: "milestone_payments");

            migrationBuilder.DropColumn(
                name: "CreatedBy",
                table: "milestone_participations");

            migrationBuilder.DropColumn(
                name: "UpdatedBy",
                table: "milestone_participations");

            migrationBuilder.DropColumn(
                name: "CreatedBy",
                table: "employees");

            migrationBuilder.DropColumn(
                name: "UpdatedBy",
                table: "employees");

            migrationBuilder.DropColumn(
                name: "CreatedBy",
                table: "employee_vacation_balances");

            migrationBuilder.DropColumn(
                name: "UpdatedBy",
                table: "employee_vacation_balances");

            migrationBuilder.DropColumn(
                name: "CreatedBy",
                table: "employee_aguinaldo_balances");

            migrationBuilder.DropColumn(
                name: "UpdatedBy",
                table: "employee_aguinaldo_balances");

            migrationBuilder.DropColumn(
                name: "CreatedBy",
                table: "deduction_rules");

            migrationBuilder.DropColumn(
                name: "UpdatedBy",
                table: "deduction_rules");

            migrationBuilder.DropColumn(
                name: "CreatedBy",
                table: "deduction_payments");

            migrationBuilder.DropColumn(
                name: "UpdatedBy",
                table: "deduction_payments");

            migrationBuilder.DropColumn(
                name: "CreatedBy",
                table: "base_salary_rules");

            migrationBuilder.DropColumn(
                name: "UpdatedBy",
                table: "base_salary_rules");

            migrationBuilder.DropColumn(
                name: "CreatedBy",
                table: "aguinaldo_rules");

            migrationBuilder.DropColumn(
                name: "UpdatedBy",
                table: "aguinaldo_rules");

            migrationBuilder.DropColumn(
                name: "CreatedBy",
                table: "aguinaldo_payments");

            migrationBuilder.DropColumn(
                name: "UpdatedBy",
                table: "aguinaldo_payments");

            migrationBuilder.DropColumn(
                name: "CreatedBy",
                table: "activity_productivity_weights");

            migrationBuilder.DropColumn(
                name: "UpdatedBy",
                table: "activity_productivity_weights");
        }
    }
}
