using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Application.Features.Analytics.DTOs;
using Application.Features.Analytics.Queries;
using Domain.Features.Employees.Interfaces;
using Domain.Features.Payrolls.Interfaces;
using Domain.Features.TimeEntries.Interfaces;

namespace Application.Features.Analytics.Handlers
{
    public class GetProjectCostsHandler
    {
        private readonly ITimeEntryRepository _timeEntryRepository;
        private readonly IEmployeeRepository _employeeRepository;
        private readonly IBaseSalaryRuleRepository _baseSalaryRuleRepository;

        public GetProjectCostsHandler(
            ITimeEntryRepository timeEntryRepository,
            IEmployeeRepository employeeRepository,
            IBaseSalaryRuleRepository baseSalaryRuleRepository)
        {
            _timeEntryRepository = timeEntryRepository;
            _employeeRepository = employeeRepository;
            _baseSalaryRuleRepository = baseSalaryRuleRepository;
        }

        public async Task<List<ProjectCostDto>> Handle(GetProjectCostsQuery query, CancellationToken ct)
        {
            var periodStart = DateTime.SpecifyKind(query.PeriodStart, DateTimeKind.Utc);
            var periodEnd = DateTime.SpecifyKind(query.PeriodEnd, DateTimeKind.Utc);

            var entries = await _timeEntryRepository.GetByPeriodAsync(periodStart, periodEnd);
            entries = entries.Where(e => e.Reviewed && e.ApprovedHours.HasValue).ToList();

            if (query.ProjectId.HasValue)
                entries = entries.Where(e => e.RedmineProjectId == query.ProjectId.Value).ToList();

            var projectGroups = entries.GroupBy(e => e.RedmineProjectId);

            var employees = await _employeeRepository.GetAllActiveAsync();
            var employeeDict = employees.ToDictionary(e => e.Id);

            var baseSalaryRules = await _baseSalaryRuleRepository.GetAllAsync();
            var hourlyRateCache = new Dictionary<Guid, decimal>();

            var result = new List<ProjectCostDto>();

            foreach (var group in projectGroups)
            {
                var projectId = group.Key;
                var projectName = group.First().ActivityName ?? "Unknown"; 

                var contributions = new List<EmployeeContributionDto>();
                decimal totalHours = 0;
                decimal totalCost = 0;

                foreach (var entry in group)
                {
                    var employeeId = entry.EmployeeId;
                    if (!hourlyRateCache.TryGetValue(employeeId, out var hourlyRate))
                    {
                        if (employeeDict.TryGetValue(employeeId, out var emp))
                        {
                            var rule = baseSalaryRules.FirstOrDefault(r => r.Role == emp.Role && r.IsActive);
                            if (rule != null)
                            {
                                hourlyRate = rule.Amount / 160m;
                            }
                        }
                        hourlyRateCache[employeeId] = hourlyRate;
                    }

                    var hours = entry.ApprovedHours!.Value;
                    var cost = hours * hourlyRate;

                    contributions.Add(new EmployeeContributionDto
                    {
                        EmployeeId = employeeId,
                        EmployeeName = employeeDict.TryGetValue(employeeId, out var e) ? e.FullName : "Unknown",
                        Hours = hours,
                        Cost = cost
                    });

                    totalHours += hours;
                    totalCost += cost;
                }

                result.Add(new ProjectCostDto
                {
                    RedmineProjectId = projectId,
                    ProjectName = projectName,
                    TotalHours = totalHours,
                    EstimatedCost = totalCost,
                    Contributions = contributions
                });
            }

            return result;
        }
    }
}