using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Domain.Interfaces;
using Domain.Models;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Persistence.Repositories
{
    public class PayrollRepository : IPayrollRepository
    {
        private readonly ApiDbContext _context;

        public PayrollRepository(ApiDbContext context)
        {
            _context = context;
        }

        public async Task AddAsync(Payroll payroll)
        {
            await _context.Payrolls.AddAsync(payroll);
            await _context.SaveChangesAsync();
        }

        public async Task<bool> ExistsAsync(Guid employeeId, DateTime from, DateTime to)
        {
            return await _context.Payrolls
                .AnyAsync(p =>
                    p.EmployeeId == employeeId &&
                    p.PeriodFrom == from &&
                    p.PeriodTo == to);
        }

        public async Task<Payroll?> GetByIdAsync(Guid id)
        {
            return await _context.Payrolls
                .Include(p => p.Lines)
                .Include(p => p.Components)
                .FirstOrDefaultAsync(p => p.Id == id);
        }

        public async Task UpdateAsync(Payroll payroll)
        {
            _context.Payrolls.Update(payroll);
            await _context.SaveChangesAsync();
        }
    }
}