using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Domain.Interfaces;
using Domain.Models;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Persistence.Repositories
{
    public class TimeEntryRepository : ITimeEntryRepository
    {
        private readonly ApiDbContext _context;

        public TimeEntryRepository(ApiDbContext context)
        {
            _context = context;
        }

        public async Task AddRangeAsync(List<TimeEntry> entries)
        {
            await _context.Set<TimeEntry>().AddRangeAsync(entries);
            await _context.SaveChangesAsync();
        }

        public async Task<bool> ExistsByRedmineIdAsync(int redmineId)
        {
            return await _context.Set<TimeEntry>().AnyAsync(x => x.RedmineTimeEntryId == redmineId);
        }

        public async Task<List<TimeEntry>> GetByEmployeeAndPeriodAsync(Guid employeeId, DateTime from, DateTime to)
        {
            return await _context.Set<TimeEntry>()
                .Where(x =>
                    x.EmployeeId == employeeId &&
                    x.SpentOn >= from &&
                    x.SpentOn <= to)
                .AsNoTracking()
                .ToListAsync();
        }

        public async Task<List<int>> GetExistingRedmineIdsAsync(List<int> redmineIds)
        {
            return await _context.Set<TimeEntry>()
                .Where(x => redmineIds.Contains(x.RedmineTimeEntryId))
                .Select(x => x.RedmineTimeEntryId)
                .ToListAsync();
        }

    }
}