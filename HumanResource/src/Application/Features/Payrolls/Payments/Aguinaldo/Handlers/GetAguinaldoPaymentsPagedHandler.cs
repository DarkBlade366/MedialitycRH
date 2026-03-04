using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Application.Common;
using Application.Features.Payrolls.Payments.Aguinaldo.DTOs;
using Application.Features.Payrolls.Payments.Aguinaldo.Queries;
using Domain.Features.Payrolls.Interfaces;

namespace Application.Features.Payrolls.Payments.Aguinaldo.Handlers
{
    public class GetAguinaldoPaymentsPagedHandler
    {
        private readonly IAguinaldoPaymentRepository _repository;

        public GetAguinaldoPaymentsPagedHandler(IAguinaldoPaymentRepository repository)
        {
            _repository = repository;
        }

        public async Task<PagedResponse<AguinaldoPaymentResponse>> HandleAsync(
            GetAguinaldoPaymentsPagedQuery query)
        {
            var payments = await _repository.GetAllAsync();

            if (query.PayrollId.HasValue)
                payments = payments
                    .Where(p => p.PayrollId == query.PayrollId.Value)
                    .ToList();

            if (query.AguinaldoRuleId.HasValue)
                payments = payments
                    .Where(p => p.AguinaldoRuleId == query.AguinaldoRuleId.Value)
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
                .Select(p => new AguinaldoPaymentResponse
                {
                    Id = p.Id,
                    PayrollId = p.PayrollId,
                    AguinaldoRuleId = p.AguinaldoRuleId,
                    Amount = p.Amount,
                    PaidAt = p.PaidAt
                })
                .ToList();

            var totalPages = (int)Math.Ceiling(totalItems / (double)query.PageSize);

            return new PagedResponse<AguinaldoPaymentResponse>
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
