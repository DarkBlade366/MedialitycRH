using System;
using System.Linq;
using System.Threading.Tasks;
using Application.Common.Interfaces;
using Application.Features.Payrolls.Payroll.DTOs;
using Application.Features.Payrolls.Payroll.Queries;
using Domain.Features.Payrolls.Interfaces;

namespace Application.Features.Payrolls.Payroll.Handlers
{
    public class GetPayrollByIdHandler
    {
        private readonly IPayrollRepository _repository;
        private readonly ICacheService _cache;

        public GetPayrollByIdHandler(IPayrollRepository repository, ICacheService cache)
        {
            _repository = repository;
            _cache = cache;
        }

        public async Task<PayrollResponse?> HandleAsync(GetPayrollByIdQuery query)
        {
            string cacheKey = $"payroll:{query.Id}";
            var cached = await _cache.GetAsync<PayrollResponse>(cacheKey);
            if (cached != null)
                return cached;

            var payroll = await _repository.GetByIdAsync(query.Id);

            if (payroll == null)
                return null;

            var response = new PayrollResponse
            {
                Id = payroll.Id,
                EmployeeId = payroll.EmployeeId,
                PeriodStart = payroll.PeriodStart,
                PeriodEnd = payroll.PeriodEnd,
                GrossAmount = payroll.GrossAmount,
                TotalDeductions = payroll.TotalDeductions,
                NetAmount = payroll.NetAmount,
                Status = payroll.Status.ToString(),
                Components = payroll.Components
                    .Select(c => new PayrollComponentResponse
                    {
                        Type = c.Type.ToString(),
                        Category = c.Category.ToString(),
                        Description = c.Description,
                        Amount = c.Amount
                    })
                    .ToList()
            };

            await _cache.SetAsync(cacheKey, response, TimeSpan.FromMinutes(10));

            return response;
        }
    }
}