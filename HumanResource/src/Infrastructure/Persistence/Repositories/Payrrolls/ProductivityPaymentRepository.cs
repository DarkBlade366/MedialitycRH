using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Domain.Features.Payrolls.Aggregates.Payments;
using Domain.Features.Payrolls.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Persistence.Repositories.Payrrolls
{
    public class ProductivityPaymentRepository : IProductivityPaymentRepository
    {
        private readonly ApiDbContext _context;

        public ProductivityPaymentRepository(ApiDbContext context)
        {
            _context = context;
        }

        public async Task<List<ProductivityPayment>> GetAllAsync()
        {
            return await _context.ProductivityPayments
                .AsNoTracking()
                .ToListAsync();
        }
    }
}
