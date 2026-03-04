using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Domain.Features.Payrolls.Aggregates.Payments;

namespace Domain.Features.Payrolls.Interfaces
{
    public interface IProductivityPaymentRepository
    {
        Task<List<ProductivityPayment>> GetAllAsync();
    }
}
