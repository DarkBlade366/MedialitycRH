using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Application.Redmine;
using Domain.Interfaces;
using Domain.Models;

namespace Application.Redmine
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
                if (!employee.RedmineUserId.HasValue)
                    continue;

                var entries = await _redmineService.GetTimeEntriesAsync(from, to, employee.RedmineUserId);
                var list = new List<TimeEntry>();

                foreach (var e in entries)
                {
                    var exists = await _timeRepository.ExistsByRedmineIdAsync(e.Id);

                    if (exists)
                        continue;

                    list.Add(new TimeEntry(
                        e.Id,
                        employee.Id,
                        e.Hours,
                        e.SpentOn,
                        e.Project.Name));
                }

                if (list.Count > 0)
                    await _timeRepository.AddRangeAsync(list);
            }
        }
    }
}