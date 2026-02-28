using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Infrastructure.Persistence;
using Infrastructure.Security;
using Application.Common.Security;
using Infrastructure.Redmine;
// using Infrastructure.Reports;
using Application.Features.Redmine.Interfaces;
using Domain.Features.Employees.Interfaces;
using Domain.Features.TimeEntries.Interfaces;
using Domain.Features.Payrolls.Interfaces;
using Domain.Features.Projects.Interfaces;
using Infrastructure.Persistence.Repositories.Projects;
using Infrastructure.Persistence.Repositories.Payrrolls;
using Infrastructure.Persistence.Repositories.TimeEntries;
using Infrastructure.Persistence.Repositories.Employees;
using Infrastructure.Persistence.Repositories;
using Application.Features.Payrolls.Interfaces;
using Application.Common.Interfaces;

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
            services.AddScoped<IProjectRepository, ProjectRepository>();
            services.AddScoped<IPayrollRepository, PayrollRepository>();
            services.AddScoped<IDeductionRuleRepository, DeductionRuleRepository>();
            services.AddScoped<IMilestoneRuleRepository, MilestoneRuleRepository>();
            services.AddScoped<IOvertimeRuleRepository, OvertimeRuleRepository>();
            services.AddScoped<IProductivityRuleRepository, ProductivityRuleRepository>();
            services.AddScoped<IVacationRuleRepository, VacationRuleRepository>();
            services.AddScoped<IBaseSalaryRuleRepository, BaseSalaryRuleRepository>();
            services.AddScoped<IAguinaldoRuleRepository, AguinaldoRuleRepository>();

            // Unit of Work
            services.AddScoped<IUnitOfWork, UnitOfWork>();

            //Register other services (e.g., token generator) if needed
            services.AddScoped<ITokenGenerator, JwtTokenGenerator>();
            //services.AddScoped<IPayrollPdfGenerator, PayrollPdfGenerator>();

            return services;
        }
    }
}