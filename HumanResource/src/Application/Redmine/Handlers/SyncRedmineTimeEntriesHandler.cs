using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Application.Redmine;
using Application.Redmine.Interfaces;
using Domain.Interfaces;
using Domain.Models;

namespace Application.Redmine.Handlers
{
    public class SyncRedmineTimeEntriesHandler
    {
        private readonly IRedmineService _redmineService;
        private readonly IEmployeeRepository _employeeRepository;
        private readonly ITimeEntryRepository _timeRepository;

        public SyncRedmineTimeEntriesHandler(IRedmineService redmineService, IEmployeeRepository employeeRepository, ITimeEntryRepository timeRepository)
        {
            _redmineService = redmineService;
            _employeeRepository = employeeRepository;
            _timeRepository = timeRepository;
        }

        public async Task Handle(DateTime from, DateTime to)
        {
            var employees = await _employeeRepository.GetPagedAsync(1, 1000);

            foreach (var employee in employees.Item1)
            {
                var entries = await _redmineService.GetTimeEntriesAsync(from, to, employee.RedmineUserId);
                var list = new List<TimeEntry>();

                var redmineIds = entries.Select(x => x.Id).ToList();
                var existingIds = await _timeRepository.GetExistingRedmineIdsAsync(redmineIds);
                var existingIdsSet = existingIds.ToHashSet();

                foreach (var e in entries)
                {
                    if (existingIdsSet.Contains(e.Id))
                        continue;

                    var spentOnUtc = e.SpentOn.Kind == DateTimeKind.Utc
                        ? e.SpentOn
                        : DateTime.SpecifyKind(e.SpentOn, DateTimeKind.Utc);

                    list.Add(new TimeEntry(
                        e.Id,
                        e.Project.Id,
                        employee.Id,
                        e.Hours,
                        spentOnUtc,
                        e.Project.Name));
                }

                if (list.Count > 0)
                    await _timeRepository.AddRangeAsync(list);
            }
        }
    }
}