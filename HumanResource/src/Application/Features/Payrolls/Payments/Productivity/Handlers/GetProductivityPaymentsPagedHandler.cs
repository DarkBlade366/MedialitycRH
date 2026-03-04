using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Application.Common;
using Application.Features.Payrolls.Payments.Productivity.DTOs;
using Application.Features.Payrolls.Payments.Productivity.Queries;
using Domain.Features.Payrolls.Interfaces;

namespace Application.Features.Payrolls.Payments.Productivity.Handlers
{
    public class GetProductivityPaymentsPagedHandler
    {
        private readonly IProductivityPaymentRepository _repository;

        public GetProductivityPaymentsPagedHandler(IProductivityPaymentRepository repository)
        {
            _repository = repository;
        }

        public async Task<PagedResponse<ProductivityPaymentResponse>> HandleAsync(
            GetProductivityPaymentsPagedQuery query)
        {
            var payments = await _repository.GetAllAsync();

            if (query.PayrollId.HasValue)
                payments = payments
                    .Where(p => p.PayrollId == query.PayrollId.Value)
                    .ToList();

            if (query.ProductivityRuleId.HasValue)
                payments = payments
                    .Where(p => p.ProductivityRuleId == query.ProductivityRuleId.Value)
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
                .Select(p => new ProductivityPaymentResponse
                {
                    Id = p.Id,
                    PayrollId = p.PayrollId,
                    ProductivityRuleId = p.ProductivityRuleId,
                    Amount = p.Amount,
                    PaidAt = p.PaidAt
                })
                .ToList();

            var totalPages = (int)Math.Ceiling(totalItems / (double)query.PageSize);

            return new PagedResponse<ProductivityPaymentResponse>
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
