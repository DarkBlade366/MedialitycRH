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
    public class GetHoursComparisonHandler
    {
        private readonly ITimeEntryRepository _timeEntryRepository;
        private readonly IEmployeeRepository _employeeRepository;
        private readonly IOvertimeRuleRepository _overtimeRuleRepository;

        public GetHoursComparisonHandler(
            ITimeEntryRepository timeEntryRepository,
            IEmployeeRepository employeeRepository,
            IOvertimeRuleRepository overtimeRuleRepository)
        {
            _timeEntryRepository = timeEntryRepository;
            _employeeRepository = employeeRepository;
            _overtimeRuleRepository = overtimeRuleRepository;
        }

        public async Task<HoursComparisonDto> Handle(GetHoursComparisonQuery query, CancellationToken ct)
        {
            var employee = await _employeeRepository.GetByIdAsync(query.EmployeeId);

            if (employee == null)
                throw new KeyNotFoundException("Employee not found.");

            var periodStart = DateTime.SpecifyKind(query.PeriodStart, DateTimeKind.Utc);
            var periodEnd = DateTime.SpecifyKind(query.PeriodEnd, DateTimeKind.Utc);

            var registered = await _timeEntryRepository.GetWorkedHours(
                query.EmployeeId, periodStart, periodEnd);

            var overtimeRule = (await _overtimeRuleRepository.GetAllAsync())
                .FirstOrDefault(r => r.IsActive);

            var expected = overtimeRule?.StandardHoursPerPeriod ?? 160m;

            var difference = registered - expected;
            var percentage = expected > 0 ? (registered / expected) * 100 : 0;

            return new HoursComparisonDto
            {
                EmployeeId = employee.Id,
                EmployeeName = employee.FullName,
                PeriodStart = query.PeriodStart,
                PeriodEnd = query.PeriodEnd,
                RegisteredHours = registered,
                ExpectedHours = expected,
                Difference = difference,
                Percentage = Math.Round(percentage, 2)
            };
        }
    }
}