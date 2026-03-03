using Microsoft.Extensions.DependencyInjection;
using Application.Features.Employees.Handlers;
using Application.Features.Employees.Validations;
using FluentValidation;
using Application.Auth.Handlers;
using Application.Auth.Validations;
using Application.Features.Redmine.Handlers;
using Application.Features.TimeEntries.Handlers;
using Application.Features.TimeEntries.Validations;
using Application.Features.Projects.Handlers;
using Application.Features.Projects.Validations;
using Application.Features.Milestones.Handlers;
using Application.Features.Milestones.Validations;
using Application.Features.Payrolls.Rules.Milestones.Handlers;
using Application.Features.Payrolls.Rules.Milestones.Validations;
using Application.Features.Payrolls.Rules.Aguinaldo.Handlers;
using Application.Features.Payrolls.Rules.Aguinaldo.Validations;
using Application.Features.Payrolls.Rules.BaseSalary.Handlers;
using Application.Features.Payrolls.Rules.BaseSalary.Validations;
using Application.Features.Payrolls.Payroll.Validations;
using Application.Features.Payrolls.Rules.Overtime.Handlers;
using Application.Features.Payrolls.Rules.Overtime.Validations;
using Application.Features.Payrolls.Rules.Productivity.Handlers;
using Application.Features.Payrolls.Rules.Productivity.Validations;
using Application.Features.Payrolls.Rules.Deduction.Handlers;
using Application.Features.Payrolls.Rules.Deduction.Validations;
using Application.Features.Payrolls.Rules.Vacation.Validations;
using Application.Features.Payrolls.Rules.Vacation.Handlers;
using Application.Services;
using Application.Features.Payrolls.Payroll.Handlers;

namespace Application
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddApplication(this IServiceCollection services)
        {
            // Register handlers
            services.AddScoped<CreateEmployeeHandler>();
            services.AddScoped<LoginHandler>();
            services.AddScoped<GetEmployeesHandler>();
            services.AddScoped<GetEmployeeByIdHandler>();
            services.AddScoped<ChangeEmployeeStatusHandler>();
            services.AddScoped<SyncRedmineTimeEntriesHandler>();
            services.AddScoped<SyncRedmineUsersHandler>();
            services.AddScoped<SyncRedmineProjectsHandler>();
            services.AddScoped<ListTimeEntriesHandler>();
            services.AddScoped<GetProjectByIdHandler>();
            services.AddScoped<ListPagedTimeEntriesHandler>();
            services.AddScoped<GetProjectsPagedHandler>();
            services.AddScoped<GetEmployeeByRedmineUserIdHandler>();
            services.AddScoped<SyncRedmineMilestonesHandler>();
            services.AddScoped<GetMilestoneByIdHandler>();
            services.AddScoped<GetMilestonesPagedHandler>();
            services.AddScoped<ChangeMilestoneRuleStatusHandler>();
            services.AddScoped<CreateMilestoneRuleHandler>();
            services.AddScoped<GetMilestoneRuleByIdHandler>();
            services.AddScoped<GetMilestoneRulesPagedHandler>();
            services.AddScoped<ChangeAguinaldoRuleStatusHandler>();
            services.AddScoped<CreateAguinaldoRuleHandler>();
            services.AddScoped<GetAguinaldoRuleByIdHandler>();
            services.AddScoped<GetAguinaldoRulesPagedHandler>();
            services.AddScoped<CreateBaseSalaryRuleHandler>();
            services.AddScoped<ChangeBaseSalaryRuleStatusHandler>();
            services.AddScoped<GetBaseSalaryRuleByIdHandler>();
            services.AddScoped<GetBaseSalaryRulesPagedHandler>();
            services.AddScoped<CreateOvertimeRuleHandler>();
            services.AddScoped<ChangeOvertimeRuleStatusHandler>();
            services.AddScoped<GetOvertimeRuleByIdHandler>();
            services.AddScoped<GetOvertimeRulesPagedHandler>();
            services.AddScoped<CreateProductivityRuleHandler>();
            services.AddScoped<ChangeProductivityRuleStatusHandler>();
            services.AddScoped<GetProductivityRuleByIdHandler>();
            services.AddScoped<GetProductivityRulesPagedHandler>();
            services.AddScoped<CreateDeductionRuleHandler>();
            services.AddScoped<ChangeDeductionRuleStatusHandler>();
            services.AddScoped<GetDeductionRuleByIdHandler>();
            services.AddScoped<GetDeductionRulesPagedHandler>();
            services.AddScoped<CreateVacationRuleHandler>();
            services.AddScoped<ChangeVacationRuleStatusHandler>();
            services.AddScoped<GetVacationRuleByIdHandler>();
            services.AddScoped<GetVacationRulesPagedHandler>();
            services.AddScoped<GetVacationBalanceHandler>();
            services.AddScoped<UseVacationHandler>();
            services.AddScoped<CreatePayrollHandler>();
            services.AddScoped<ChangeMilestoneParticipationStatusHandler>();
            services.AddScoped<GetMilestoneParticipationByIdHandler>();
            services.AddScoped<GetMilestoneParticipationsPagedHandler>();
            services.AddScoped<CreateMilestoneParticipationHandler>();


            // Register validators
            services.AddValidatorsFromAssemblyContaining<CreateEmployeeValidation>();
            services.AddValidatorsFromAssemblyContaining<LoginValidation>();
            services.AddValidatorsFromAssemblyContaining<GetEmployeesValidation>();
            services.AddValidatorsFromAssemblyContaining<GetEmployeeByIdValidation>();
            services.AddValidatorsFromAssemblyContaining<ChangeEmployeeStatusValidation>();
            services.AddValidatorsFromAssemblyContaining<ListTimeEntriesQueryValidator>();
            services.AddValidatorsFromAssemblyContaining<ListPagedTimeEntriesValidator>();
            services.AddValidatorsFromAssemblyContaining<GetProjectByIdValidator>();
            services.AddValidatorsFromAssemblyContaining<GetProjectsPagedValidator>();
            services.AddValidatorsFromAssemblyContaining<GetEmployeeByRedmineUserIdValidation>();
            services.AddValidatorsFromAssemblyContaining<GetMilestonesPagedValidator>();
            services.AddValidatorsFromAssemblyContaining<GetMilestoneByIdValidator>();
            services.AddValidatorsFromAssemblyContaining<GetMilestoneRulesPagedValidator>();
            services.AddValidatorsFromAssemblyContaining<CreateMilestoneRuleValidator>();
            services.AddValidatorsFromAssemblyContaining<ChangeMilestoneRuleStatusValidator>();
            services.AddValidatorsFromAssemblyContaining<CreateAguinaldoRuleValidator>();
            services.AddValidatorsFromAssemblyContaining<ChangeAguinaldoRuleStatusValidator>();
            services.AddValidatorsFromAssemblyContaining<GetAguinaldoRulesPagedValidator>();
            services.AddValidatorsFromAssemblyContaining<GetAguinaldoRuleByIdValidator>();
            services.AddValidatorsFromAssemblyContaining<CreateBaseSalaryRuleValidator>();
            services.AddValidatorsFromAssemblyContaining<ChangeBaseSalaryRuleStatusValidator>();
            services.AddValidatorsFromAssemblyContaining<GetBaseSalaryRuleByIdValidator>();
            services.AddValidatorsFromAssemblyContaining<GetBaseSalaryRulesPagedValidator>();
            services.AddValidatorsFromAssemblyContaining<CreateOvertimeRuleValidator>();
            services.AddValidatorsFromAssemblyContaining<ChangeOvertimeRuleStatusValidator>();
            services.AddValidatorsFromAssemblyContaining<GetOvertimeRuleByIdValidator>();
            services.AddValidatorsFromAssemblyContaining<GetOvertimeRulesPagedValidator>();
            services.AddValidatorsFromAssemblyContaining<CreateProductivityRuleValidator>();
            services.AddValidatorsFromAssemblyContaining<ChangeProductivityRuleStatusValidator>();
            services.AddValidatorsFromAssemblyContaining<GetProductivityRuleByIdValidator>();
            services.AddValidatorsFromAssemblyContaining<GetProductivityRulesPagedValidator>();
            services.AddValidatorsFromAssemblyContaining<CreateDeductionRuleValidator>();
            services.AddValidatorsFromAssemblyContaining<ChangeDeductionRuleStatusValidator>();
            services.AddValidatorsFromAssemblyContaining<GetDeductionRuleByIdValidator>();
            services.AddValidatorsFromAssemblyContaining<GetDeductionRulesPagedValidator>();
            services.AddValidatorsFromAssemblyContaining<CreateVacationRuleValidator>();
            services.AddValidatorsFromAssemblyContaining<ChangeVacationRuleStatusValidator>();
            services.AddValidatorsFromAssemblyContaining<GetVacationRuleByIdValidator>();
            services.AddValidatorsFromAssemblyContaining<GetVacationRulesPagedValidator>();
            services.AddValidatorsFromAssemblyContaining<GetVacationBalanceQueryValidator>();
            services.AddValidatorsFromAssemblyContaining<UseVacationCommandValidator>();
            services.AddValidatorsFromAssemblyContaining<CreatePayrollCommandValidator>();
            services.AddValidatorsFromAssemblyContaining<CreateMilestoneParticipationValidator>();
            services.AddValidatorsFromAssemblyContaining<ChangeMilestoneParticipationStatusValidator>();
            services.AddValidatorsFromAssemblyContaining<GetMilestoneParticipationByIdValidator>();
            services.AddValidatorsFromAssemblyContaining<GetMilestoneParticipationsPagedValidator>();

            //other services
            services.AddScoped<VacationAccrualService>();

            return services;
        }
    }
}