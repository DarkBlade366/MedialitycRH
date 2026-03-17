using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddPerformanceIndexes : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // ========== TimeEntries ==========
            migrationBuilder.CreateIndex(
                name: "IX_TimeEntries_EmployeeId_SpentOn",
                table: "time_entries",
                columns: new[] { "EmployeeId", "SpentOn" });

            migrationBuilder.CreateIndex(
                name: "IX_TimeEntries_Reviewed",
                table: "time_entries",
                column: "Reviewed",
                filter: "\"Reviewed\" = false");

            migrationBuilder.CreateIndex(
                name: "IX_TimeEntries_RedmineTimeEntryId",
                table: "time_entries",
                column: "RedmineTimeEntryId",
                unique: true);

            // ========== Employees ==========
            migrationBuilder.CreateIndex(
                name: "IX_Employees_Email",
                table: "employees",
                column: "Email",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Employees_RedmineUserId",
                table: "employees",
                column: "RedmineUserId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Employees_FullName",
                table: "employees",
                column: "FullName");

            // ========== Payrolls ==========
            migrationBuilder.CreateIndex(
                name: "IX_Payrolls_EmployeeId_PeriodStart_PeriodEnd",
                table: "payrolls",
                columns: new[] { "EmployeeId", "PeriodStart", "PeriodEnd" });

            migrationBuilder.CreateIndex(
                name: "IX_Payrolls_Status",
                table: "payrolls",
                column: "Status");

            // ========== ProjectMilestones ==========
            migrationBuilder.CreateIndex(
                name: "IX_ProjectMilestones_RedmineProjectId_Status",
                table: "project_milestones",
                columns: new[] { "RedmineProjectId", "Status" });

            migrationBuilder.CreateIndex(
                name: "IX_ProjectMilestones_CompletedAt",
                table: "project_milestones",
                column: "CompletedAt")
                .Annotation("Npgsql:IndexMethod", "btree");

            // ========== MilestoneParticipations ==========
            migrationBuilder.CreateIndex(
                name: "IX_MilestoneParticipations_EmployeeId",
                table: "milestone_participations",
                column: "EmployeeId");

            migrationBuilder.CreateIndex(
                name: "IX_MilestoneParticipations_ProjectMilestoneId_EmployeeId",
                table: "milestone_participations",
                columns: new[] { "ProjectMilestoneId", "EmployeeId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_MilestoneParticipations_IsPaid",
                table: "milestone_participations",
                column: "IsPaid");

            // ========== Reglas de nómina ==========

            // BaseSalaryRules
            migrationBuilder.CreateIndex(
                name: "IX_BaseSalaryRules_Role_IsActive",
                table: "base_salary_rules",
                columns: new[] { "Role", "IsActive" });

            // DeductionRules
            migrationBuilder.CreateIndex(
                name: "IX_DeductionRules_Type_IsActive",
                table: "deduction_rules",
                columns: new[] { "Type", "IsActive" });

            // OvertimeRules
            migrationBuilder.CreateIndex(
                name: "IX_OvertimeRules_IsActive",
                table: "overtime_rules",
                column: "IsActive");

            // ProductivityRules
            migrationBuilder.CreateIndex(
                name: "IX_ProductivityRules_IsActive",
                table: "productivity_rules",
                column: "IsActive");

            // VacationRules
            migrationBuilder.CreateIndex(
                name: "IX_VacationRules_IsActive",
                table: "vacation_rules",
                column: "IsActive");

            // AguinaldoRules
            migrationBuilder.CreateIndex(
                name: "IX_AguinaldoRules_IsActive",
                table: "aguinaldo_rules",
                column: "IsActive");

            // MilestoneRules
            migrationBuilder.CreateIndex(
                name: "IX_MilestoneRules_RedmineProjectId_MilestoneName_IsActive",
                table: "milestone_rules",
                columns: new[] { "RedmineProjectId", "MilestoneName", "IsActive" });

            // ProjectRules
            migrationBuilder.CreateIndex(
                name: "IX_ProjectRules_RedmineProjectId_IsActive",
                table: "project_rules",
                columns: new[] { "RedmineProjectId", "IsActive" });

            // ========== Pagos (Payments) ==========

            // AguinaldoPayments
            migrationBuilder.CreateIndex(
                name: "IX_AguinaldoPayments_PayrollId",
                table: "aguinaldo_payments",
                column: "PayrollId");

            migrationBuilder.CreateIndex(
                name: "IX_AguinaldoPayments_AguinaldoRuleId",
                table: "aguinaldo_payments",
                column: "AguinaldoRuleId");

            // DeductionPayments
            migrationBuilder.CreateIndex(
                name: "IX_DeductionPayments_PayrollId",
                table: "deduction_payments",
                column: "PayrollId");

            migrationBuilder.CreateIndex(
                name: "IX_DeductionPayments_DeductionRuleId",
                table: "deduction_payments",
                column: "DeductionRuleId");

            // MilestonePayments
            migrationBuilder.CreateIndex(
                name: "IX_MilestonePayments_PayrollId",
                table: "milestone_payments",
                column: "PayrollId");

            migrationBuilder.CreateIndex(
                name: "IX_MilestonePayments_MilestoneRuleId",
                table: "milestone_payments",
                column: "MilestoneRuleId");

            // OvertimePayments
            migrationBuilder.CreateIndex(
                name: "IX_OvertimePayments_PayrollId",
                table: "overtime_payments",
                column: "PayrollId");

            migrationBuilder.CreateIndex(
                name: "IX_OvertimePayments_OvertimeRuleId",
                table: "overtime_payments",
                column: "OvertimeRuleId");

            // ProductivityPayments
            migrationBuilder.CreateIndex(
                name: "IX_ProductivityPayments_PayrollId",
                table: "productivity_payments",
                column: "PayrollId");

            migrationBuilder.CreateIndex(
                name: "IX_ProductivityPayments_ProductivityRuleId",
                table: "productivity_payments",
                column: "ProductivityRuleId");

            // ProjectPayments
            migrationBuilder.CreateIndex(
                name: "IX_ProjectPayments_PayrollId",
                table: "project_payments",
                column: "PayrollId");

            migrationBuilder.CreateIndex(
                name: "IX_ProjectPayments_RedmineProjectId",
                table: "project_payments",
                column: "RedmineProjectId");

            // VacationPayments
            migrationBuilder.CreateIndex(
                name: "IX_VacationPayments_PayrollId",
                table: "vacation_payments",
                column: "PayrollId");

            migrationBuilder.CreateIndex(
                name: "IX_VacationPayments_VacationRuleId",
                table: "vacation_payments",
                column: "VacationRuleId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            // TimeEntries
            migrationBuilder.DropIndex(name: "IX_TimeEntries_EmployeeId_SpentOn", table: "time_entries");
            migrationBuilder.DropIndex(name: "IX_TimeEntries_Reviewed", table: "time_entries");
            migrationBuilder.DropIndex(name: "IX_TimeEntries_RedmineTimeEntryId", table: "time_entries");

            // Employees
            migrationBuilder.DropIndex(name: "IX_Employees_Email", table: "employees");
            migrationBuilder.DropIndex(name: "IX_Employees_RedmineUserId", table: "employees");
            migrationBuilder.DropIndex(name: "IX_Employees_FullName", table: "employees");

            // Payrolls
            migrationBuilder.DropIndex(name: "IX_Payrolls_EmployeeId_PeriodStart_PeriodEnd", table: "payrolls");
            migrationBuilder.DropIndex(name: "IX_Payrolls_Status", table: "payrolls");

            // ProjectMilestones
            migrationBuilder.DropIndex(name: "IX_ProjectMilestones_RedmineProjectId_Status", table: "project_milestones");
            migrationBuilder.DropIndex(name: "IX_ProjectMilestones_CompletedAt", table: "project_milestones");

            // MilestoneParticipations
            migrationBuilder.DropIndex(name: "IX_MilestoneParticipations_EmployeeId", table: "milestone_participations");
            migrationBuilder.DropIndex(name: "IX_MilestoneParticipations_ProjectMilestoneId_EmployeeId", table: "milestone_participations");
            migrationBuilder.DropIndex(name: "IX_MilestoneParticipations_IsPaid", table: "milestone_participations");

            // Reglas
            migrationBuilder.DropIndex(name: "IX_BaseSalaryRules_Role_IsActive", table: "base_salary_rules");
            migrationBuilder.DropIndex(name: "IX_DeductionRules_Type_IsActive", table: "deduction_rules");
            migrationBuilder.DropIndex(name: "IX_OvertimeRules_IsActive", table: "overtime_rules");
            migrationBuilder.DropIndex(name: "IX_ProductivityRules_IsActive", table: "productivity_rules");
            migrationBuilder.DropIndex(name: "IX_VacationRules_IsActive", table: "vacation_rules");
            migrationBuilder.DropIndex(name: "IX_AguinaldoRules_IsActive", table: "aguinaldo_rules");
            migrationBuilder.DropIndex(name: "IX_MilestoneRules_RedmineProjectId_MilestoneName_IsActive", table: "milestone_rules");
            migrationBuilder.DropIndex(name: "IX_ProjectRules_RedmineProjectId_IsActive", table: "project_rules");

            // Pagos
            migrationBuilder.DropIndex(name: "IX_AguinaldoPayments_PayrollId", table: "aguinaldo_payments");
            migrationBuilder.DropIndex(name: "IX_AguinaldoPayments_AguinaldoRuleId", table: "aguinaldo_payments");
            migrationBuilder.DropIndex(name: "IX_DeductionPayments_PayrollId", table: "deduction_payments");
            migrationBuilder.DropIndex(name: "IX_DeductionPayments_DeductionRuleId", table: "deduction_payments");
            migrationBuilder.DropIndex(name: "IX_MilestonePayments_PayrollId", table: "milestone_payments");
            migrationBuilder.DropIndex(name: "IX_MilestonePayments_MilestoneRuleId", table: "milestone_payments");
            migrationBuilder.DropIndex(name: "IX_OvertimePayments_PayrollId", table: "overtime_payments");
            migrationBuilder.DropIndex(name: "IX_OvertimePayments_OvertimeRuleId", table: "overtime_payments");
            migrationBuilder.DropIndex(name: "IX_ProductivityPayments_PayrollId", table: "productivity_payments");
            migrationBuilder.DropIndex(name: "IX_ProductivityPayments_ProductivityRuleId", table: "productivity_payments");
            migrationBuilder.DropIndex(name: "IX_ProjectPayments_PayrollId", table: "project_payments");
            migrationBuilder.DropIndex(name: "IX_ProjectPayments_RedmineProjectId", table: "project_payments");
            migrationBuilder.DropIndex(name: "IX_VacationPayments_PayrollId", table: "vacation_payments");
            migrationBuilder.DropIndex(name: "IX_VacationPayments_VacationRuleId", table: "vacation_payments");
        }
    }
}