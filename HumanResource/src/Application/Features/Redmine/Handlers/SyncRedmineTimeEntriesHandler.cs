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
        private readonly ICacheService _cache;

        public SyncRedmineTimeEntriesHandler(
            IRedmineService redmineService,
            IEmployeeRepository employeeRepository,
            ITimeEntryRepository timeRepository,
            IUnitOfWork unitOfWork,
            ICacheService cache)
        {
            _redmineService = redmineService;
            _employeeRepository = employeeRepository;
            _timeRepository = timeRepository;
            _unitOfWork = unitOfWork;
            _cache = cache;
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

                var existingEntries = await _timeRepository.GetByRedmineIdsAsync(redmineIds);
                var existingDict = existingEntries.ToDictionary(x => x.RedmineTimeEntryId);

                var listToAdd = new List<TimeEntry>();

                foreach (var e in entries)
                {
                    var spentOnUtc = e.SpentOn.Kind == DateTimeKind.Utc
                        ? e.SpentOn
                        : DateTime.SpecifyKind(e.SpentOn, DateTimeKind.Utc);

                    var activityId = e.Activity?.Id > 0
                        ? e.Activity.Id
                        : (int?)null;

                    var activityName = !string.IsNullOrWhiteSpace(e.Activity?.Name)
                        ? e.Activity.Name
                        : null;

                    if (!existingDict.TryGetValue(e.Id, out var existing))
                    {
                        listToAdd.Add(new TimeEntry(
                            e.Id,
                            e.Project.Id,
                            employee.Id,
                            e.Hours,
                            spentOnUtc,
                            activityId,
                            activityName));

                        created++;
                    }
                    else
                    {
                        existing.Update(
                            e.Hours,
                            spentOnUtc,
                            activityId,
                            activityName
                        );
                        _timeRepository.Update(existing);
                    }
                }

                if (listToAdd.Count > 0)
                    await _timeRepository.AddRangeAsync(listToAdd);
            }

            await _unitOfWork.SaveChangesAsync(ct);

            //No invalidamos actualmente, pq son muchos los posibles cambios, dejo inyectado el cache para futuro anadido si se quiere

            return created;
        }
    }
}