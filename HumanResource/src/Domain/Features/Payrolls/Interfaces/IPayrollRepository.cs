using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Domain.Features.Payrolls.Aggregates;
using Domain.Features.Payrolls.Enums;

namespace Domain.Features.Payrolls.Interfaces
{
    public interface IPayrollRepository
    {
        Task<Payroll?> GetByIdAsync(Guid id);
        Task<Payroll?> GetByEmployeeAndPeriodAsync(
            Guid employeeId,
            DateTime periodStart,
            DateTime periodEnd);
        Task<IReadOnlyCollection<Payroll>> GetByEmployeeAsync(Guid employeeId);
        Task<bool> ExistsForPeriodAsync(
                Guid employeeId,
                DateTime periodStart,
                DateTime periodEnd);

        public Task<bool> ExistsOverlappingPayroll(
            Guid employeeId,
            DateTime periodStart,
            DateTime periodEnd);
        Task AddAsync(Payroll payroll);
    
        void Update(Payroll payroll);
    
    }
}