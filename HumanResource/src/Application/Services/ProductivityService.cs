using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Domain.Features.Payrolls.Interfaces;
using Domain.Features.TimeEntries.Interfaces;

namespace Application.Services
{
    public class ProductivityService
    {
        private readonly ITimeEntryRepository _timeEntryRepository;
        private readonly IActivityProductivityWeightRepository _activityWeightRepository;

        public ProductivityService(
            ITimeEntryRepository timeEntryRepository,
            IActivityProductivityWeightRepository activityWeightRepository)
        {
            _timeEntryRepository = timeEntryRepository;
            _activityWeightRepository = activityWeightRepository;
        }
        public async Task<decimal> CalculateProductivityMetric(
            Guid employeeId,
            DateTime periodStart,
            DateTime periodEnd)
        {
            var hoursByActivity = await _timeEntryRepository.GetHoursByActivityAsync(
                employeeId, periodStart, periodEnd);

            if (hoursByActivity.Count == 0)
                return 0m;

            var weights = (await _activityWeightRepository.GetAllAsync())
                .Where(w => w.IsActive)
                .ToDictionary(w => w.RedmineActivityId, w => w.Weight);

            decimal weightedSum = 0m;

            foreach (var (activityId, hours) in hoursByActivity)
            {
                var weight = activityId == 0 || !weights.TryGetValue(activityId, out var w)
                    ? 1m
                    : w;

                weightedSum += hours * weight;
            }

            return weightedSum;
        }
    }
}