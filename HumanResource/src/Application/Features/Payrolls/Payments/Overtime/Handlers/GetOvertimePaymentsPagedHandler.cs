using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Application.Common;
using Application.Common.Interfaces;
using Application.Features.Payrolls.Payments.Overtime.DTOs;
using Application.Features.Payrolls.Payments.Overtime.Queries;
using Domain.Features.Payrolls.Aggregates.Payments;
using Domain.Features.Payrolls.Interfaces;

namespace Application.Features.Payrolls.Payments.Overtime.Handlers
{
    public class GetOvertimePaymentsPagedHandler
    {
        private readonly IOvertimePaymentRepository _repository;
        private readonly ICacheService _cache;

        public GetOvertimePaymentsPagedHandler(IOvertimePaymentRepository repository, ICacheService cache)
        {
            _repository = repository;
            _cache = cache;
        }

        public async Task<PagedResponse<OvertimePaymentResponse>> HandleAsync(GetOvertimePaymentsPagedQuery query)
        {
            string cacheKey = "overtimePayments:all";
            var payments = await _cache.GetAsync<List<OvertimePayment>>(cacheKey);
            if (payments == null)
            {
                payments = (await _repository.GetAllAsync())?.ToList() ?? new List<OvertimePayment>();
                await _cache.SetAsync(cacheKey, payments, TimeSpan.FromMinutes(10));
            }

            var filtered = payments.AsEnumerable();

            if (query.PayrollId.HasValue)
                filtered = filtered.Where(p => p.PayrollId == query.PayrollId.Value);

            if (query.OvertimeRuleId.HasValue)
                filtered = filtered.Where(p => p.OvertimeRuleId == query.OvertimeRuleId.Value);

            if (query.From.HasValue)
                filtered = filtered.Where(p => p.PaidAt >= query.From.Value);

            if (query.To.HasValue)
                filtered = filtered.Where(p => p.PaidAt <= query.To.Value);

            var ordered = filtered.OrderByDescending(p => p.PaidAt).ToList();
            var totalItems = ordered.Count;

            var items = ordered
                .Skip((query.Page - 1) * query.PageSize)
                .Take(query.PageSize)
                .Select(p => new OvertimePaymentResponse
                {
                    Id = p.Id,
                    PayrollId = p.PayrollId,
                    OvertimeRuleId = p.OvertimeRuleId,
                    Amount = p.Amount,
                    PaidAt = p.PaidAt
                })
                .ToList();

            var totalPages = (int)Math.Ceiling(totalItems / (double)query.PageSize);

            return new PagedResponse<OvertimePaymentResponse>
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