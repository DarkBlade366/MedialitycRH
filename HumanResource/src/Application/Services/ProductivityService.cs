using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Domain.Features.TimeEntries.Interfaces;

namespace Application.Services
{
    public class ProductivityService
    {
        private readonly ITimeEntryRepository _timeEntryRepository;

        public ProductivityService(ITimeEntryRepository timeEntryRepository)
        {
            _timeEntryRepository = timeEntryRepository;
        }

        public async Task<decimal> CalculateProductivityMetric(
            Guid employeeId,
            DateTime periodStart,
            DateTime periodEnd)
        {
            var hours = await _timeEntryRepository.GetWorkedHours(
                employeeId,
                periodStart,
                periodEnd);

            return hours;
        }
    }
}