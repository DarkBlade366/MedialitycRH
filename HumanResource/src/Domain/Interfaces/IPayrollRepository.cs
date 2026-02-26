using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Domain.Models;

namespace Domain.Interfaces
{
    public interface IPayrollRepository
    {
        Task AddAsync(Payroll payroll);
        Task<bool> ExistsAsync(Guid employeeId, DateTime from, DateTime to);
        Task<Payroll?> GetByIdAsync(Guid id);
        Task UpdateAsync(Payroll payroll);
    }
}