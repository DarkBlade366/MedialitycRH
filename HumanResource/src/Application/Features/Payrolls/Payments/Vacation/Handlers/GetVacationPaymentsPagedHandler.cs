using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Application.Common;
using Application.Features.Payrolls.Payments.Vacation.DTOs;
using Application.Features.Payrolls.Payments.Vacation.Queries;
using Domain.Features.Payrolls.Interfaces;

namespace Application.Features.Payrolls.Payments.Vacation.Handlers
{
    public class GetVacationPaymentsPagedHandler
    {
        private readonly IVacationPaymentRepository _repository;

        public GetVacationPaymentsPagedHandler(IVacationPaymentRepository repository)
        {
            _repository = repository;
        }

        public async Task<PagedResponse<VacationPaymentResponse>> HandleAsync(
            GetVacationPaymentsPagedQuery query)
        {
            var payments = await _repository.GetAllAsync();

            if (query.PayrollId.HasValue)
                payments = payments
                    .Where(p => p.PayrollId == query.PayrollId.Value)
                    .ToList();

            if (query.VacationRuleId.HasValue)
                payments = payments
                    .Where(p => p.VacationRuleId == query.VacationRuleId.Value)
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
