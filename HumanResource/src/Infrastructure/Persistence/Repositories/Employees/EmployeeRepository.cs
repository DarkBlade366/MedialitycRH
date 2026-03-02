using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using Domain.Features.Employees.Interfaces;
using Domain.Features.Employees.Aggregates;

namespace Infrastructure.Persistence.Repositories.Employees
{
    public class EmployeeRepository : IEmployeeRepository
    {
        private readonly ApiDbContext _context;

        public EmployeeRepository(ApiDbContext context)
        {
            _context = context;
        }

        public async Task AddAsync(Employee employee)
        {
            await _context.Employees.AddAsync(employee);
        }

        public async Task<Employee?> GetByIdAsync(Guid id)
        {
            return await _context.Employees
                .Include(e => e.AguinaldoBalance)
                .Include(e => e.VacationBalance)
                .FirstOrDefaultAsync(e => e.Id == id);
        }

        public async Task<Employee?> GetByEmailAsync(string email)
        {
            return await _context.Employees
                .Include(e => e.AguinaldoBalance)
                .Include(e => e.VacationBalance)
                .FirstOrDefaultAsync(e => e.Email == email);
        }

        public async Task<(IReadOnlyList<Employee>, int)> GetPagedAsync(int page, int pageSize)
        {
            var query = _context.Employees.AsNoTracking();

            var totalCount = await query.CountAsync();

            var items = await query
                .OrderBy(e => e.FullName)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            return (items, totalCount);
        }

        public void UpdateAsync(Employee employee)
        {
            _context.Employees.Update(employee);
        }

        public async Task<List<string>> GetExistingEmailsAsync(List<string> emails)
        {
            return await _context.Set<Employee>()
                .Where(x => emails.Contains(x.Email))
                .Select(x => x.Email)
                .ToListAsync();
        }

        public async Task AddRangeAsync(List<Employee> employees)
        {
            await _context.Set<Employee>().AddRangeAsync(employees);
        }

        public async Task<bool> ExistsByRedmineUserIdAsync(int redmineUserId)
        {
            return await _context.Employees
                .AnyAsync(x => x.RedmineUserId == redmineUserId);
        }

        public async Task<bool> ExistsByEmailAsync(string email)
        {
            return await _context.Employees
                .AnyAsync(e => e.Email == email);
        }

        public async Task<Employee?> GetByIdWithBalancesAsync(Guid id)
        {
            return await _context.Employees
                .Include(e => e.VacationBalance)
                .Include(e => e.AguinaldoBalance)
                .FirstOrDefaultAsync(e => e.Id == id);
        }

        public async Task<Employee?> GetByRedmineUserIdAsync(int redmineUserId)
        {
            return await _context.Employees
                .Include(e => e.VacationBalance)
                .Include(e => e.AguinaldoBalance)
                .FirstOrDefaultAsync(e => e.RedmineUserId == redmineUserId);
        }

        public async Task<List<Employee>> GetByRedmineIdsAsync(HashSet<int> redmineIds)
        {
            return await _context.Employees
                .Where(e => redmineIds.Contains(e.RedmineUserId))
                .ToListAsync();
        }

        public async Task<List<Employee>> GetAllActiveAsync()
        {
            return await _context.Employees
                .Where(e => e.IsActive)
                .Include(e => e.VacationBalance)
                .Include(e => e.AguinaldoBalance)
                .ToListAsync();
        }
    }
}
