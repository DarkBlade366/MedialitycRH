using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Application.Common;
using Application.Features.Payrolls.Payments.Milestone.DTOs;
using Application.Features.Payrolls.Payments.Milestone.Queries;
using Domain.Features.Payrolls.Interfaces;

namespace Application.Features.Payrolls.Payments.Milestone.Handlers
{
    public class GetMilestonePaymentsPagedHandler
    {
        private readonly IMilestonePaymentRepository _repository;

        public GetMilestonePaymentsPagedHandler(IMilestonePaymentRepository repository)
        {
            _repository = repository;
        }

        public async Task<PagedResponse<MilestonePaymentResponse>> HandleAsync(
            GetMilestonePaymentsPagedQuery query)
        {
            var payments = await _repository.GetAllAsync();

            if (query.PayrollId.HasValue)
                payments = payments
                    .Where(p => p.PayrollId == query.PayrollId.Value)
                    .ToList();

            if (query.MilestoneRuleId.HasValue)
                payments = payments
                    .Where(p => p.MilestoneRuleId == query.MilestoneRuleId.Value)
                    .ToList();

            if (query.From.HasValue)
                payments = payments
                    .Where(p => p.PaidAt >= query.From.Value)
                    .ToList();

            if (query.To.HasValue)
                payments = payments
                    .Where(p => p.PaidAt <= query.To.Value)
                    .ToList();

            var orderedPayments = payments
                .OrderByDescending(p => p.PaidAt)
                .ToList();

            var totalItems = orderedPayments.Count;

            var items = orderedPayments
                .Skip((query.Page - 1) * query.PageSize)
                .Take(query.PageSize)
                .Select(p => new MilestonePaymentResponse
                {
                    Id = p.Id,
                    PayrollId = p.PayrollId,
                    MilestoneRuleId = p.MilestoneRuleId,
                    Amount = p.Amount,
                    PaidAt = p.PaidAt
                })
                .ToList();

            var totalPages = (int)Math.Ceiling(totalItems / (double)query.PageSize);

            return new PagedResponse<MilestonePaymentResponse>
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
