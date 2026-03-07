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
    public class GetProductivityTrendHandler
    {
        private readonly ITimeEntryRepository _timeEntryRepository;
        private readonly IActivityProductivityWeightRepository _weightRepository;
        private readonly IEmployeeRepository _employeeRepository;

        public GetProductivityTrendHandler(
            ITimeEntryRepository timeEntryRepository,
            IActivityProductivityWeightRepository weightRepository,
            IEmployeeRepository employeeRepository)
        {
            _timeEntryRepository = timeEntryRepository;
            _weightRepository = weightRepository;
            _employeeRepository = employeeRepository;
        }

        public async Task<List<ProductivityTrendDto>> Handle(GetProductivityTrendQuery query, CancellationToken ct)
        {
            var employee = await _employeeRepository.GetByIdAsync(query.EmployeeId);

            if (employee == null)
                throw new KeyNotFoundException("Employee not found.");

            var weights = (await _weightRepository.GetAllAsync())
                .Where(w => w.IsActive)
                .ToDictionary(w => w.RedmineActivityId, w => w.Weight);

            var months = new List<(int Year, int Month)>();
            var current = new DateTime(query.From.Year, query.From.Month, 1);
            var end = new DateTime(query.To.Year, query.To.Month, 1);

            while (current <= end)
            {
                months.Add((current.Year, current.Month));
                current = current.AddMonths(1);
            }

            var result = new List<ProductivityTrendDto>();

            foreach (var (year, month) in months)
            {
                var monthStart = new DateTime(year, month, 1, 0, 0, 0, DateTimeKind.Utc);
                var monthEnd = monthStart.AddMonths(1).AddDays(-1).AddHours(23).AddMinutes(59).AddSeconds(59);

                var hoursByActivity = await _timeEntryRepository.GetHoursByActivityAsync(
                    query.EmployeeId, monthStart, monthEnd);

                if (hoursByActivity.Count == 0)
                {
                    result.Add(new ProductivityTrendDto { Year = year, Month = month, Metric = 0 });
                    continue;
                }

                decimal weightedSum = 0;
                
                foreach (var (activityId, hours) in hoursByActivity)
                {
                    var weight = activityId == 0 || !weights.TryGetValue(activityId, out var w) ? 1m : w;
                    weightedSum += hours * weight;
                }

                result.Add(new ProductivityTrendDto
                {
                    Year = year,
                    Month = month,
                    Metric = Math.Round(weightedSum, 2)
                });
            }

            return result;
        }
    }
}