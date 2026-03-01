using Microsoft.Extensions.DependencyInjection;
using Application.Features.Employees.Handlers;
using Application.Features.Employees.Validations;
using FluentValidation;
using Application.Auth.Handlers;
using Application.Auth.Validations;
using Application.Features.Redmine.Handlers;
using Application.Features.TimeEntries.Handlers;
using Application.Features.TimeEntries.Validations;
using Application.Features.Payrolls.Validations;
using Application.Features.Projects.Handlers;
using Application.Features.Projects.Validations;
using Application.Features.Milestones.Handlers;
using Application.Features.Milestones.Validations;
using Application.Features.Payrolls.Rules.Milestones.Handlers;
using Application.Features.Payrolls.Rules.Milestones.Validations;

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
            // services.AddScoped<GeneratePayrollHandler>();
            // services.AddScoped<ApprovePayrollHandler>();
            // services.AddScoped<GetPayrollPdfHandler>();


            // Register validators
            services.AddValidatorsFromAssemblyContaining<CreateEmployeeValidation>();
            services.AddValidatorsFromAssemblyContaining<LoginValidation>();
            services.AddValidatorsFromAssemblyContaining<GetEmployeesValidation>();
            services.AddValidatorsFromAssemblyContaining<GetEmployeeByIdValidation>();
            services.AddValidatorsFromAssemblyContaining<ChangeEmployeeStatusValidation>();
            services.AddValidatorsFromAssemblyContaining<ListTimeEntriesQueryValidator>();
            services.AddValidatorsFromAssemblyContaining<ListPagedTimeEntriesValidator>();
            services.AddValidatorsFromAssemblyContaining<GeneratePayrollValidator>();
            services.AddValidatorsFromAssemblyContaining<ApprovePayrollValidation>();
            services.AddValidatorsFromAssemblyContaining<GetProjectByIdValidator>();
            services.AddValidatorsFromAssemblyContaining<GetProjectsPagedValidator>();
            services.AddValidatorsFromAssemblyContaining<GetEmployeeByRedmineUserIdValidation>();
            services.AddValidatorsFromAssemblyContaining<GetMilestoneByIdValidator>();
            services.AddValidatorsFromAssemblyContaining<GetMilestonesPagedValidator>();
            services.AddValidatorsFromAssemblyContaining<GetMilestoneRulesPagedValidator>();
            services.AddValidatorsFromAssemblyContaining<CreateMilestoneRuleValidator>();


            return services;
        }
    }
}