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
    }
}