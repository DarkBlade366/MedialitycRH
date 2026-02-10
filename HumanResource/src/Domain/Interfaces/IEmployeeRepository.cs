using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Domain.Models;

namespace Domain.Interfaces
{
    public interface IEmployeeRepository
    {
        Task AddAsync(Employee employee);
        Task<Employee?> GetByIdAsync(Guid id);

        Task<Employee?> GetByEmailAsync(string email);

        Task<(IReadOnlyList<Employee>, int totalCount)> GetPagedAsync(int page, int pageSize);
        Task UpdateAsync(Employee employee);
    }
}