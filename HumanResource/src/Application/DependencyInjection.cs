using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Application.Employees.Handlers;
using Application.Employees.Validations;
using FluentValidation;

namespace Application
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddApplication(this IServiceCollection services)
        {
            // Register handlers
            services.AddScoped<CreateEmployeeHandler>();

            // Register validators
            services.AddValidatorsFromAssemblyContaining<CreateEmployeeValidation>();

            return services;
        }
    }
}