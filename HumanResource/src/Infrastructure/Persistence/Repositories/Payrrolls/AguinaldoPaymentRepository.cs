using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Domain.Features.Payrolls.Aggregates.Payments;
using Domain.Features.Payrolls.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Persistence.Repositories.Payrrolls
{
    public class AguinaldoPaymentRepository : IAguinaldoPaymentRepository
    {
        private readonly ApiDbContext _context;

        public AguinaldoPaymentRepository(ApiDbContext context)
        {
            _context = context;
        }

        public async Task<List<AguinaldoPayment>> GetAllAsync()
        {
            return await _context.AguinaldoPayments
                .AsNoTracking()
                .ToListAsync();
        }
    }
}
