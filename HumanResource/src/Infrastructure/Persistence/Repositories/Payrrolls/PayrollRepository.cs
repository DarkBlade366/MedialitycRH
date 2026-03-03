using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Domain.Features.Payrolls.Aggregates;
using Domain.Features.Payrolls.Interfaces;

namespace Infrastructure.Persistence.Repositories.Payrrolls
{
    public class PayrollRepository : IPayrollRepository
    {
        private readonly ApiDbContext _context;

        public PayrollRepository(ApiDbContext context)
        {
            _context = context;
        }

        public async Task<Payroll?> GetByIdAsync(Guid id)
        {
            return await _context.Payrolls
                .Include(p => p.Components)
                .Include(p => p.MilestonePayments)
                .Include(p => p.AguinaldoPayments)
                .Include(p => p.VacationPayments)
                .Include(p => p.ProductivityPayments)
                .Include(p => p.OvertimePayments)
                .FirstOrDefaultAsync(p => p.Id == id);
        }

        public async Task<Payroll?> GetByEmployeeAndPeriodAsync(
            Guid employeeId,
            DateTime periodStart,
            DateTime periodEnd)
        {
            return await _context.Payrolls
                .Include(p => p.Components)
                .Include(p => p.MilestonePayments)
                .Include(p => p.AguinaldoPayments)
                .Include(p => p.VacationPayments)
                .Include(p => p.ProductivityPayments)
                .Include(p => p.OvertimePayments)
                .FirstOrDefaultAsync(p =>
                    p.EmployeeId == employeeId &&
                    p.PeriodStart == periodStart &&
                    p.PeriodEnd == periodEnd);
        }

        public async Task<IReadOnlyCollection<Payroll>> GetByEmployeeAsync(Guid employeeId)
        {
            return await _context.Payrolls
                .Where(p => p.EmployeeId == employeeId)
                .ToListAsync();
        }

        public async Task<bool> ExistsForPeriodAsync(
            Guid employeeId,
            DateTime periodStart,
            DateTime periodEnd)
        {
            return await _context.Payrolls.AnyAsync(p =>
                p.EmployeeId == employeeId &&
                p.PeriodStart == periodStart &&
                p.PeriodEnd == periodEnd);
        }

        public async Task AddAsync(Payroll payroll)
        {
            await _context.Payrolls.AddAsync(payroll);
        }

        public void Update(Payroll payroll)
        {
            _context.Payrolls.Update(payroll);
        }

        public void Remove(Payroll payroll)
        {
            _context.Payrolls.Remove(payroll);
        }

        public async Task<bool> ExistsOverlappingPayroll(
            Guid employeeId,
            DateTime periodStart,
            DateTime periodEnd)
        {
            return await _context.Payrolls
                .AnyAsync(p =>
                    p.EmployeeId == employeeId &&
                    periodStart <= p.PeriodEnd &&
                    periodEnd >= p.PeriodStart);
        }
    }
}
