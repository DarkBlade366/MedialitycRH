using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Domain.Features.TimeEntries.Aggregates;

namespace Domain.Features.TimeEntries.Interfaces
{
    public interface ITimeEntryRepository
    {
        Task AddRangeAsync(IEnumerable<TimeEntry> entries);
        Task<bool> ExistsByRedmineIdAsync(int redmineTimeEntryId);
        Task<List<int>> GetExistingRedmineIdsAsync(IEnumerable<int> redmineIds);

        Task<List<TimeEntry>> GetByEmployeeAndPeriodAsync(
            Guid employeeId,
            DateTime from,
            DateTime to);

        Task<(List<TimeEntry> Items, int TotalCount)> GetPagedFilteredAsync(
            Guid? employeeId,
            DateTime? from,
            DateTime? to,
            int page,
            int pageSize);

        Task<decimal> GetWorkedHours(
            Guid employeeId,
            DateTime periodStart,
            DateTime periodEnd);

        Task<Dictionary<int, decimal>> GetHoursByActivityAsync(
            Guid employeeId,
            DateTime periodStart,
            DateTime periodEnd);

        Task<List<TimeEntry>> GetByRedmineIdsAsync(IEnumerable<int> redmineIds);
        Task<bool> HasPendingEntries(Guid employeeId, DateTime start, DateTime end);
        Task<TimeEntry?> GetByIdAsync(Guid id);
        Task<List<TimeEntry>> GetByIdsAsync(IEnumerable<Guid> ids);
        Task<List<TimeEntry>> GetByPeriodAsync(
            DateTime periodStart,
            DateTime periodEnd);
        void Update(TimeEntry entry);
    }
}