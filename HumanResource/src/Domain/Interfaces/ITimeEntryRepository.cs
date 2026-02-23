    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using Domain.Models;

    namespace Domain.Interfaces
    {
        public interface ITimeEntryRepository
        {
            Task AddRangeAsync(List<TimeEntry> entries);
            Task<bool> ExistsByRedmineIdAsync(int redmineId);
            Task<List<TimeEntry>> GetByEmployeeAndPeriodAsync(Guid employeeId, DateTime from, DateTime to);
        }
    }