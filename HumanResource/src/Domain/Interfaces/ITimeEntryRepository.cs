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

            //actualmente no se usa, pero podría ser útil para evitar consultas masivas
            Task<bool> ExistsByRedmineIdAsync(int redmineId); 
            
            Task<List<TimeEntry>> GetByEmployeeAndPeriodAsync(Guid employeeId, DateTime from, DateTime to);
            public Task<List<int>> GetExistingRedmineIdsAsync(List<int> redmineIds);
        }
    }