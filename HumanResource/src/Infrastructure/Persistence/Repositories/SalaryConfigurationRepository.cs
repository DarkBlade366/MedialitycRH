using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Domain.Enums;
using Domain.Interfaces;
using Domain.Models;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Persistence.Repositories
{
    public class SalaryConfigurationRepository : ISalaryConfigurationRepository
    {
        private readonly ApiDbContext _context;

        public SalaryConfigurationRepository(ApiDbContext context)
        {
            _context = context;
        }

        public async Task<SalaryConfiguration?> GetByRoleAsync(EmployeeRole role)
        {
            return await _context.SalaryConfigurations
                .FirstOrDefaultAsync(x => x.Role == role);
        }
    }
}