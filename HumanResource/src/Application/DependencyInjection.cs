using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Application.Employees.Handlers;
using Application.Employees.Validations;
using FluentValidation;
using Application.Auth.Handlers;
using Application.Auth.Validations;
using Application.Redmine.Handlers;
using Application.TimeEntries.Handlers;
using Application.TimeEntries.Validations;
//using Application.Projects.Handlers;
//using Application.Projects.Validations;
using Application.Payrolls.Handlers;
using Application.Payrolls.Validations;
using Application.SalaryConfigurations.Handlers;

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
            services.AddScoped<UpdateEmployeeHandler>();
            services.AddScoped<ChangeEmployeeStatusHandler>();
            services.AddScoped<ChangeEmployeeRedmineUserIdHandler>();
            services.AddScoped<SyncRedmineTimeEntriesHandler>();
            services.AddScoped<SyncRedmineUsersHandler>();
            services.AddScoped<SyncRedmineProjectsHandler>();
            services.AddScoped<ListTimeEntriesHandler>();
            services.AddScoped<ListPagedTimeEntriesHandler>();
            services.AddScoped<GeneratePayrollHandler>();
            services.AddScoped<ApprovePayrollHandler>();
            services.AddScoped<GetPayrollPdfHandler>();
            services.AddScoped<SyncSalaryConfigurationsHandler>();
            services.AddScoped<UpdateSalaryConfigurationHandler>();


            // Register validators
            services.AddValidatorsFromAssemblyContaining<CreateEmployeeValidation>();
            services.AddValidatorsFromAssemblyContaining<LoginValidation>();
            services.AddValidatorsFromAssemblyContaining<GetEmployeesValidation>();
            services.AddValidatorsFromAssemblyContaining<GetEmployeeByIdValidation>(); 
            services.AddValidatorsFromAssemblyContaining<UpdateEmployeeValidation>();
            services.AddValidatorsFromAssemblyContaining<ChangeEmployeeStatusValidation>();
            services.AddValidatorsFromAssemblyContaining<ChangeEmployeeRedmineUserIdValidator>();
            services.AddValidatorsFromAssemblyContaining<ListTimeEntriesQueryValidator>();
            services.AddValidatorsFromAssemblyContaining<ListPagedTimeEntriesValidator>();
            services.AddValidatorsFromAssemblyContaining<GeneratePayrollValidator>();
            services.AddValidatorsFromAssemblyContaining<ApprovePayrollValidation>();

            return services;
        }
    }
}