using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Domain.Features.Employees.Aggregates;

namespace Domain.Features.Employees.Interfaces
{
    public interface IEmployeeRepository
    {
        Task AddAsync(Employee employee);
        Task AddRangeAsync(List<Employee> employees);

        Task<Employee?> GetByIdAsync(Guid id);
        Task<Employee?> GetByEmailAsync(string email);
        Task<Employee?> GetByIdWithBalancesAsync(Guid id);
        Task<Employee?> GetByRedmineUserIdAsync(int redmineUserId);

        Task<(IReadOnlyList<Employee>, int totalCount)> GetPagedAsync(int page, int pageSize);

        Task<List<string>> GetExistingEmailsAsync(List<string> emails);
        Task<bool> ExistsByEmailAsync(string email);

        public Task<bool> ExistsByRedmineUserIdAsync(int redmineUserId);
        

        void UpdateAsync(Employee employee);

        Task<List<Employee>> GetByRedmineIdsAsync(HashSet<int> redmineIds);
        Task<List<Employee>> GetAllActiveAsync();
    }
}
