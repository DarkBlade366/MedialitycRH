using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Domain.Features.TimeEntries.Aggregates;
using Domain.Features.TimeEntries.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Persistence.Repositories.TimeEntries
{
    public class TimeEntryRepository : ITimeEntryRepository
    {
        private readonly ApiDbContext _context;

        public TimeEntryRepository(ApiDbContext context)
        {
            _context = context;
        }

        public async Task AddRangeAsync(IEnumerable<TimeEntry> entries)
        {
            await _context.TimeEntries.AddRangeAsync(entries);
        }

        public async Task<bool> ExistsByRedmineIdAsync(int redmineTimeEntryId)
        {
            return await _context.TimeEntries
                .AnyAsync(x => x.RedmineTimeEntryId == redmineTimeEntryId);
        }

        public async Task<List<int>> GetExistingRedmineIdsAsync(IEnumerable<int> redmineIds)
        {
            return await _context.TimeEntries
                .Where(x => redmineIds.Contains(x.RedmineTimeEntryId))
                .Select(x => x.RedmineTimeEntryId)
                .ToListAsync();
        }

        public async Task<List<TimeEntry>> GetByEmployeeAndPeriodAsync(
            Guid employeeId,
            DateTime from,
            DateTime to)
        {
            return await _context.TimeEntries
                .Where(x =>
                    x.EmployeeId == employeeId &&
                    x.SpentOn >= from &&
                    x.SpentOn <= to)
                .AsNoTracking()
                .ToListAsync();
        }

        public async Task<(List<TimeEntry> Items, int TotalCount)> GetPagedFilteredAsync(
            Guid? employeeId,
            DateTime? from,
            DateTime? to,
            int page,
            int pageSize)
        {
            var query = _context.TimeEntries.AsQueryable();

            if (employeeId.HasValue)
                query = query.Where(x => x.EmployeeId == employeeId.Value);

            if (from.HasValue)
                query = query.Where(x => x.SpentOn >= from.Value);

            if (to.HasValue)
                query = query.Where(x => x.SpentOn <= to.Value);

            var totalCount = await query.CountAsync();

            var items = await query
                .OrderByDescending(x => x.SpentOn)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .AsNoTracking()
                .ToListAsync();

            return (items, totalCount);
        }
    }
}