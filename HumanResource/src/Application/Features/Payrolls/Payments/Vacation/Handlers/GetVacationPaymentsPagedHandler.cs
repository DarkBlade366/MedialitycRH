using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Application.Common;
using Application.Common.Interfaces;
using Application.Features.Payrolls.Payments.Vacation.DTOs;
using Application.Features.Payrolls.Payments.Vacation.Queries;
using Domain.Features.Payrolls.Aggregates.Payments;
using Domain.Features.Payrolls.Interfaces;

namespace Application.Features.Payrolls.Payments.Vacation.Handlers
{
    public class GetVacationPaymentsPagedHandler
    {
        private readonly IVacationPaymentRepository _repository;
        private readonly ICacheService _cache;

        public GetVacationPaymentsPagedHandler(IVacationPaymentRepository repository, ICacheService cache)
        {
            _repository = repository;
            _cache = cache;
        }

        public async Task<PagedResponse<VacationPaymentResponse>> HandleAsync(GetVacationPaymentsPagedQuery query)
        {
            string cacheKey = "vacationPayments:all";
            var payments = await _cache.GetAsync<List<VacationPayment>>(cacheKey);
            if (payments == null)
            {
                payments = (await _repository.GetAllAsync())?.ToList() ?? new List<VacationPayment>();
                await _cache.SetAsync(cacheKey, payments, TimeSpan.FromMinutes(10));
            }

            var filtered = payments.AsEnumerable();

            if (query.PayrollId.HasValue)
                filtered = filtered.Where(p => p.PayrollId == query.PayrollId.Value);

            if (query.VacationRuleId.HasValue)
                filtered = filtered.Where(p => p.VacationRuleId == query.VacationRuleId.Value);

            if (query.From.HasValue)
                filtered = filtered.Where(p => p.PaidAt >= query.From.Value);

            if (query.To.HasValue)
                filtered = filtered.Where(p => p.PaidAt <= query.To.Value);

            var ordered = filtered.OrderByDescending(p => p.PaidAt).ToList();
            var totalItems = ordered.Count;

            var items = ordered
                .Skip((query.Page - 1) * query.PageSize)
                .Take(query.PageSize)
                .Select(p => new VacationPaymentResponse
                {
                    Id = p.Id,
                    PayrollId = p.PayrollId,
                    VacationRuleId = p.VacationRuleId,
                    Amount = p.Amount,
                    PaidAt = p.PaidAt
                })
                .ToList();

            var totalPages = (int)Math.Ceiling(totalItems / (double)query.PageSize);

            return new PagedResponse<VacationPaymentResponse>
            {
                Items = items,
                Page = query.Page,
                PageSize = query.PageSize,
                TotalItems = totalItems,
                TotalPages = totalPages
            };
        }
    }
}