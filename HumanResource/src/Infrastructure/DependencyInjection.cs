using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Infrastructure.Persistence;
using Polly;
using Polly.Extensions.Http;
using Infrastructure.Security;
using Application.Common.Security;
using Infrastructure.Redmine;
using Infrastructure.Reports;
using Application.Features.Redmine.Interfaces;
using Domain.Features.Employees.Interfaces;
using Domain.Features.TimeEntries.Interfaces;
using Domain.Features.Payrolls.Interfaces;
using Domain.Features.Projects.Interfaces;
using Infrastructure.Persistence.Repositories.Projects;
using Infrastructure.Persistence.Repositories.Payrrolls;
using Infrastructure.Persistence.Repositories.TimeEntries;
using Infrastructure.Persistence.Repositories.Employees;    
using Application.Common.Interfaces;
using Infrastructure.Interfaces;
using Infrastructure.Services;

namespace Infrastructure
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
        {
            // DbContext
            services.AddDbContext<ApiDbContext>((serviceProvider, options) =>
            {
                var interceptor = serviceProvider.GetRequiredService<AuditInterceptor>();
                options.UseNpgsql(configuration.GetConnectionString("DbMedialitycHR"))
                        .AddInterceptors(interceptor);
            });

            //Redmine
            services.AddHttpClient<IRedmineService, RedmineClient>(client =>
            {
                var baseUrl = configuration["Redmine:BaseUrl"]
                    ?? throw new InvalidOperationException("Redmine BaseUrl not configured");

                var apiKey = configuration["Redmine:ApiKey"]
                    ?? throw new InvalidOperationException("Redmine ApiKey not configured");

                client.BaseAddress = new Uri(baseUrl);

                client.DefaultRequestHeaders.Add("X-Redmine-API-Key", apiKey);
            })
            .AddPolicyHandler(GetRedmineRetryPolicy());

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
            services.AddScoped<IAguinaldoPaymentRepository, AguinaldoPaymentRepository>();
            services.AddScoped<IDeductionPaymentRepository, DeductionPaymentRepository>();
            services.AddScoped<IMilestonePaymentRepository, MilestonePaymentRepository>();
            services.AddScoped<IOvertimePaymentRepository, OvertimePaymentRepository>();
            services.AddScoped<IProductivityPaymentRepository, ProductivityPaymentRepository>();
            services.AddScoped<IVacationPaymentRepository, VacationPaymentRepository>();
            services.AddScoped<IProjectMilestoneRepository, ProjectMilestoneRepository>();
            services.AddScoped<IMilestoneParticipationRepository, MilestoneParticipationRepository>();
            services.AddScoped<IActivityProductivityWeightRepository, ActivityProductivityWeightRepository>();
            services.AddScoped<IProjectPaymentRepository, ProjectPaymentRepository>();
            services.AddScoped<IProjectRuleRepository, ProjectRuleRepository>();

            // Unit of Work
            services.AddScoped<IUnitOfWork, UnitOfWork>();

            //Register other services (e.g., token generator) if needed
            services.AddScoped<ITokenGenerator, JwtTokenGenerator>();
            services.AddScoped<IPayrollPdfGenerator, PayrollPdfGenerator>();
            services.AddScoped<IPayrollExcelGenerator, PayrollExcelGenerator>();

            //Audi
            services.AddScoped<AuditInterceptor>();


            return services;
        }

        private static IAsyncPolicy<HttpResponseMessage> GetRedmineRetryPolicy()
        {
            return HttpPolicyExtensions
                .HandleTransientHttpError()
                .OrResult(msg => msg.StatusCode == System.Net.HttpStatusCode.TooManyRequests)
                .WaitAndRetryAsync(
                    retryCount: 3,
                    sleepDurationProvider: retryAttempt => TimeSpan.FromSeconds(Math.Pow(2, retryAttempt)),
                    onRetry: (outcome, timespan, retryCount, _) =>
                    {
                        // Logging se hace en el caller cuando falla definitivamente
                    });
        }
    }
}