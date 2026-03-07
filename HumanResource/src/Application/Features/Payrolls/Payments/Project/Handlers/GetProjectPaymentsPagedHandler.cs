using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Application.Common;
using Application.Features.Payrolls.Payments.Project.DTOs;
using Application.Features.Payrolls.Payments.Project.Queries;
using Domain.Features.Payrolls.Interfaces;

namespace Application.Features.Payrolls.Payments.Project.Handlers
{
    public class GetProjectPaymentsPagedHandler
    {
        private readonly IProjectPaymentRepository _repository;

        public GetProjectPaymentsPagedHandler(IProjectPaymentRepository repository)
        {
            _repository = repository;
        }

        public async Task<PagedResponse<ProjectPaymentResponse>> HandleAsync(
            GetProjectPaymentsPagedQuery query)
        {
            var payments = await _repository.GetAllAsync();

            if (query.PayrollId.HasValue)
                payments = payments
                    .Where(p => p.PayrollId == query.PayrollId.Value)
                    .ToList();

            if (query.RedmineProjectId.HasValue)
                payments = payments
                    .Where(p => p.RedmineProjectId == query.RedmineProjectId.Value)
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
                .Select(p => new ProjectPaymentResponse
                {
                    Id = p.Id,
                    PayrollId = p.PayrollId,
                    RedmineProjectId = p.RedmineProjectId,
                    Amount = p.Amount,
                    PaidAt = p.PaidAt
                })
                .ToList();

            var totalPages = (int)Math.Ceiling(totalItems / (double)query.PageSize);

            return new PagedResponse<ProjectPaymentResponse>
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
