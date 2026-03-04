using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Application.Common;
using Application.Features.Payrolls.Payments.Overtime.DTOs;
using Application.Features.Payrolls.Payments.Overtime.Queries;
using Domain.Features.Payrolls.Interfaces;

namespace Application.Features.Payrolls.Payments.Overtime.Handlers
{
    public class GetOvertimePaymentsPagedHandler
    {
        private readonly IOvertimePaymentRepository _repository;

        public GetOvertimePaymentsPagedHandler(IOvertimePaymentRepository repository)
        {
            _repository = repository;
        }

        public async Task<PagedResponse<OvertimePaymentResponse>> HandleAsync(
            GetOvertimePaymentsPagedQuery query)
        {
            var payments = await _repository.GetAllAsync();

            if (query.PayrollId.HasValue)
                payments = payments
                    .Where(p => p.PayrollId == query.PayrollId.Value)
                    .ToList();

            if (query.OvertimeRuleId.HasValue)
                payments = payments
                    .Where(p => p.OvertimeRuleId == query.OvertimeRuleId.Value)
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
