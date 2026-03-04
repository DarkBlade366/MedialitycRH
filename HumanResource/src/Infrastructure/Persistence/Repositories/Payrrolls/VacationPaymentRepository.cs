using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Domain.Features.Payrolls.Aggregates.Payments;
using Domain.Features.Payrolls.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Persistence.Repositories.Payrrolls
{
    public class VacationPaymentRepository : IVacationPaymentRepository
    {
        private readonly ApiDbContext _context;

        public VacationPaymentRepository(ApiDbContext context)
        {
            _context = context;
        }

        public async Task<List<VacationPayment>> GetAllAsync()
        {
            return await _context.VacationPayments
                .AsNoTracking()
                .ToListAsync();
        }
    }
}
