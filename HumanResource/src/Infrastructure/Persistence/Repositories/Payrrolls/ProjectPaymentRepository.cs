using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Domain.Features.Payrolls.Aggregates.Payments;
using Domain.Features.Payrolls.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Persistence.Repositories.Payrrolls
{
    public class ProjectPaymentRepository : IProjectPaymentRepository
    {
        private readonly ApiDbContext _context;

        public ProjectPaymentRepository(ApiDbContext context)
        {
            _context = context;
        }

        public async Task<List<ProjectPayment>> GetAllAsync()
        {
            return await _context.ProjectPayments
                .AsNoTracking()
                .ToListAsync();
        }
    }
}