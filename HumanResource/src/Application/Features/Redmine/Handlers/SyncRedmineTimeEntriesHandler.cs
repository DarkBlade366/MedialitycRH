using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Application.Common.Interfaces;
using Application.Features.Redmine.Interfaces;
using Domain.Features.Employees.Interfaces;
using Domain.Features.TimeEntries.Aggregates;
using Domain.Features.TimeEntries.Interfaces;

namespace Application.Features.Redmine.Handlers
{
    public class SyncRedmineTimeEntriesHandler
    {
        private readonly IRedmineService _redmineService;
        private readonly IEmployeeRepository _employeeRepository;
        private readonly ITimeEntryRepository _timeRepository;
        private readonly IUnitOfWork _unitOfWork;

        public SyncRedmineTimeEntriesHandler(
            IRedmineService redmineService,
            IEmployeeRepository employeeRepository,
            ITimeEntryRepository timeRepository,
            IUnitOfWork unitOfWork)
        {
            _redmineService = redmineService;
            _employeeRepository = employeeRepository;
            _timeRepository = timeRepository;
            _unitOfWork = unitOfWork;
        }

        public async Task<int> Handle(DateTime from, DateTime to, CancellationToken ct = default)
        {
            int created = 0;

            var employeesPaged = await _employeeRepository.GetPagedAsync(1, int.MaxValue);
            var employees = employeesPaged.Item1
                .Where(e => e.IsActive && e.RedmineUserId > 0);

            foreach (var employee in employees)
            {
                var entries = await _redmineService.GetTimeEntriesAsync(from, to, employee.RedmineUserId);

                if (entries == null || entries.Count == 0)
                    continue;

                var redmineIds = entries.Select(x => x.Id).ToList();
                var existingIds = await _timeRepository.GetExistingRedmineIdsAsync(redmineIds);
                var existingIdsSet = existingIds.ToHashSet();

                var listToAdd = new List<TimeEntry>();

                foreach (var e in entries)
                {
                    if (existingIdsSet.Contains(e.Id))
                        continue; 

                    var spentOnUtc = e.SpentOn.Kind == DateTimeKind.Utc
                        ? e.SpentOn
                        : DateTime.SpecifyKind(e.SpentOn, DateTimeKind.Utc);

                    listToAdd.Add(new TimeEntry(
                        e.Id,
                        e.Project.Id,
                        employee.Id,
                        e.Hours,
                        spentOnUtc));

                    created++;
                }

                if (listToAdd.Count > 0)
                    await _timeRepository.AddRangeAsync(listToAdd);
            }

            await _unitOfWork.SaveChangesAsync(ct);
            return created;
        }
    }
}