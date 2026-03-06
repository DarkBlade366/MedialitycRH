using Application.Common;
using Application.Features.Payrolls.Rules.ActivityProductivityWeight.DTOs;
using Application.Features.Payrolls.Rules.ActivityProductivityWeight.Queries;
using Domain.Features.Payrolls.Interfaces;

namespace Application.Features.Payrolls.Rules.ActivityProductivityWeight.Handlers
{
    public class GetActivityProductivityWeightsPagedHandler
    {
        private readonly IActivityProductivityWeightRepository _repository;

        public GetActivityProductivityWeightsPagedHandler(IActivityProductivityWeightRepository repository)
        {
            _repository = repository;
        }

        public async Task<PagedResponse<ActivityProductivityWeightResponse>> HandleAsync(
            GetActivityProductivityWeightsPagedQuery query)
        {
            var all = await _repository.GetAllAsync();

            if (query.IsActive.HasValue)
                all = all.Where(x => x.IsActive == query.IsActive.Value).ToList();

            var totalItems = all.Count;

            var paged = all
                .OrderBy(x => x.ActivityName)
                .Skip((query.Page - 1) * query.PageSize)
                .Take(query.PageSize)
                .Select(x => new ActivityProductivityWeightResponse
                {
                    Id = x.Id,
                    RedmineActivityId = x.RedmineActivityId,
                    ActivityName = x.ActivityName,
                    Weight = x.Weight,
                    IsActive = x.IsActive
                })
                .ToList();

            var totalPages = (int)Math.Ceiling(totalItems / (double)query.PageSize);

            return new PagedResponse<ActivityProductivityWeightResponse>
            {
                Items = paged,
                Page = query.Page,
                PageSize = query.PageSize,
                TotalItems = totalItems,
                TotalPages = totalPages
            };
        }
    }
}
