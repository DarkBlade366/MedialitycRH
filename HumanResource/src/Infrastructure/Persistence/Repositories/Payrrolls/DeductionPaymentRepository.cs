using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Domain.Features.Payrolls.Aggregates.Payments;
using Domain.Features.Payrolls.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Persistence.Repositories.Payrrolls
{
    public class DeductionPaymentRepository : IDeductionPaymentRepository
    {
        private readonly ApiDbContext _context;

        public DeductionPaymentRepository(ApiDbContext context)
        {
            _context = context;
        }

        public async Task<List<DeductionPayment>> GetAllAsync()
        {
            return await _context.Set<DeductionPayment>()
                .AsNoTracking()
                .ToListAsync();
        }
    }
}
