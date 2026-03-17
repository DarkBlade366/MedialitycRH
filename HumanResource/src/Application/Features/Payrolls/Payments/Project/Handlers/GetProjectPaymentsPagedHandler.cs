using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Application.Common;
using Application.Common.Interfaces;
using Application.Features.Payrolls.Payments.Project.DTOs;
using Application.Features.Payrolls.Payments.Project.Queries;
using Domain.Features.Payrolls.Aggregates.Payments;
using Domain.Features.Payrolls.Interfaces;

namespace Application.Features.Payrolls.Payments.Project.Handlers
{
    public class GetProjectPaymentsPagedHandler
    {
        private readonly IProjectPaymentRepository _repository;
        private readonly ICacheService _cache;

        public GetProjectPaymentsPagedHandler(IProjectPaymentRepository repository, ICacheService cache)
        {
            _repository = repository;
            _cache = cache;
        }

        public async Task<PagedResponse<ProjectPaymentResponse>> HandleAsync(GetProjectPaymentsPagedQuery query)
        {
            string cacheKey = "projectPayments:all";
            var payments = await _cache.GetAsync<List<ProjectPayment>>(cacheKey);
            if (payments == null)
            {
                payments = (await _repository.GetAllAsync())?.ToList() ?? new List<ProjectPayment>();
                await _cache.SetAsync(cacheKey, payments, TimeSpan.FromMinutes(10));
            }

            var filtered = payments.AsEnumerable();

            if (query.PayrollId.HasValue)
                filtered = filtered.Where(p => p.PayrollId == query.PayrollId.Value);

            if (query.RedmineProjectId.HasValue)
                filtered = filtered.Where(p => p.RedmineProjectId == query.RedmineProjectId.Value);

            if (query.From.HasValue)
                filtered = filtered.Where(p => p.PaidAt >= query.From.Value);

            if (query.To.HasValue)
                filtered = filtered.Where(p => p.PaidAt <= query.To.Value);

            var ordered = filtered.OrderByDescending(p => p.PaidAt).ToList();
            var totalItems = ordered.Count;

            var items = ordered
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