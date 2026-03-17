using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Application.Common;
using Application.Common.Interfaces;
using Application.Features.Payrolls.Payments.Deduction.DTOs;
using Application.Features.Payrolls.Payments.Deduction.Queries;
using Domain.Features.Payrolls.Aggregates.Payments;
using Domain.Features.Payrolls.Interfaces;

namespace Application.Features.Payrolls.Payments.Deduction.Handlers
{
    public class GetDeductionPaymentsPagedHandler
    {
        private readonly IDeductionPaymentRepository _repository;
        private readonly ICacheService _cache;

        public GetDeductionPaymentsPagedHandler(IDeductionPaymentRepository repository, ICacheService cache)
        {
            _repository = repository;
            _cache = cache;
        }

        public async Task<PagedResponse<DeductionPaymentResponse>> HandleAsync(GetDeductionPaymentsPagedQuery query)
        {
            string cacheKey = "deductionPayments:all";
            var payments = await _cache.GetAsync<List<DeductionPayment>>(cacheKey);
            if (payments == null)
            {
                payments = (await _repository.GetAllAsync())?.ToList() ?? new List<DeductionPayment>();
                await _cache.SetAsync(cacheKey, payments, TimeSpan.FromMinutes(10));
            }

            var filtered = payments.AsEnumerable();

            if (query.PayrollId.HasValue)
                filtered = filtered.Where(p => p.PayrollId == query.PayrollId.Value);

            if (query.DeductionRuleId.HasValue)
                filtered = filtered.Where(p => p.DeductionRuleId == query.DeductionRuleId.Value);

            if (query.From.HasValue)
                filtered = filtered.Where(p => p.AppliedAt >= query.From.Value);

            if (query.To.HasValue)
                filtered = filtered.Where(p => p.AppliedAt <= query.To.Value);

            var ordered = filtered.OrderByDescending(p => p.AppliedAt).ToList();
            var totalItems = ordered.Count;

            var items = ordered
                .Skip((query.Page - 1) * query.PageSize)
                .Take(query.PageSize)
                .Select(p => new DeductionPaymentResponse
                {
                    Id = p.Id,
                    PayrollId = p.PayrollId,
                    DeductionRuleId = p.DeductionRuleId,
                    Amount = p.Amount,
                    AppliedAt = p.AppliedAt
                })
                .ToList();

            var totalPages = (int)Math.Ceiling(totalItems / (double)query.PageSize);

            return new PagedResponse<DeductionPaymentResponse>
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