using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Application.Common;
using Application.Features.Payrolls.Payments.Deduction.DTOs;
using Application.Features.Payrolls.Payments.Deduction.Queries;
using Domain.Features.Payrolls.Interfaces;

namespace Application.Features.Payrolls.Payments.Deduction.Handlers
{
    public class GetDeductionPaymentsPagedHandler
    {
        private readonly IDeductionPaymentRepository _repository;

        public GetDeductionPaymentsPagedHandler(IDeductionPaymentRepository repository)
        {
            _repository = repository;
        }

        public async Task<PagedResponse<DeductionPaymentResponse>> HandleAsync(
            GetDeductionPaymentsPagedQuery query)
        {
            var payments = await _repository.GetAllAsync();

            if (query.PayrollId.HasValue)
                payments = payments
                    .Where(p => p.PayrollId == query.PayrollId.Value)
                    .ToList();

            if (query.DeductionRuleId.HasValue)
                payments = payments
                    .Where(p => p.DeductionRuleId == query.DeductionRuleId.Value)
                    .ToList();

            if (query.From.HasValue)
                payments = payments
                    .Where(p => p.AppliedAt >= query.From.Value)
                    .ToList();

            if (query.To.HasValue)
                payments = payments
                    .Where(p => p.AppliedAt <= query.To.Value)
                    .ToList();

            var orderedPayments = payments
                .OrderByDescending(p => p.AppliedAt)
                .ToList();

            var totalItems = orderedPayments.Count;

            var items = orderedPayments
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
