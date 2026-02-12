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

            // Register validators
            services.AddValidatorsFromAssemblyContaining<CreateEmployeeValidation>();
            services.AddValidatorsFromAssemblyContaining<LoginValidation>();
            services.AddValidatorsFromAssemblyContaining<GetEmployeesValidation>();
            services.AddValidatorsFromAssemblyContaining<GetEmployeeByIdValidation>(); 
            services.AddValidatorsFromAssemblyContaining<UpdateEmployeeValidation>();
            services.AddValidatorsFromAssemblyContaining<ChangeEmployeeStatusValidation>();
            services.AddValidatorsFromAssemblyContaining<ChangeEmployeeRedmineUserIdValidator>();

            return services;
        }
    }
}