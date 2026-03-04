using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Domain.Features.Payrolls.Aggregates.Payments;
using Domain.Features.Payrolls.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Persistence.Repositories.Payrrolls
{
    public class MilestonePaymentRepository : IMilestonePaymentRepository
    {
        private readonly ApiDbContext _context;

        public MilestonePaymentRepository(ApiDbContext context)
        {
            _context = context;
        }

        public async Task<List<MilestonePayment>> GetAllAsync()
        {
            return await _context.MilestonePayments
                .AsNoTracking()
                .ToListAsync();
        }
    }
}
