using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Domain.Features.Payrolls.Aggregates.Payments;
using Domain.Features.Payrolls.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Persistence.Repositories.Payrrolls
{
    public class OvertimePaymentRepository : IOvertimePaymentRepository
    {
        private readonly ApiDbContext _context;

        public OvertimePaymentRepository(ApiDbContext context)
        {
            _context = context;
        }

        public async Task<List<OvertimePayment>> GetAllAsync()
        {
            return await _context.OvertimePayments
                .AsNoTracking()
                .ToListAsync();
        }
    }
}
