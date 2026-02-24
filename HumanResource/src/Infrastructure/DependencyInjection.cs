using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Application.Employees;
using Domain.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Infrastructure.Persistence.Repositories;
using Infrastructure.Persistence.Configurations;
using Infrastructure.Persistence;
using Infrastructure.Security;
using Application.Common.Security;
using Infrastructure.Redmine;
using Application.Redmine;

namespace Infrastructure
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
        {
            // DbContext
            services.AddDbContext<ApiDbContext>(options =>
                options.UseNpgsql(configuration.GetConnectionString("DbMedialitycHR")));
    
            //Redmine
            services.AddHttpClient<IRedmineService, RedmineClient>(client =>
            {
                var baseUrl = configuration["Redmine:BaseUrl"]
                    ?? throw new InvalidOperationException("Redmine BaseUrl not configured");
            
                var apiKey = configuration["Redmine:ApiKey"]
                    ?? throw new InvalidOperationException("Redmine ApiKey not configured");
            
                client.BaseAddress = new Uri(baseUrl);
            
                client.DefaultRequestHeaders.Add("X-Redmine-API-Key", apiKey);
            });

            // Repositories
            services.AddScoped<IEmployeeRepository, EmployeeRepository>();
            services.AddScoped<ITimeEntryRepository, TimeEntryRepository>();

            //Register other services (e.g., token generator) if needed
            services.AddScoped<ITokenGenerator, JwtTokenGenerator>();

            return services;
        }
    }
}