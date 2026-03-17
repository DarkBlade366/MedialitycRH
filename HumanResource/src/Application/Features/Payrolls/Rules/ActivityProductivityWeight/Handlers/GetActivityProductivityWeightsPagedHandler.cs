using Application.Common;
using Application.Common.Interfaces;
using Application.Features.Payrolls.Rules.ActivityProductivityWeight.DTOs;
using Application.Features.Payrolls.Rules.ActivityProductivityWeight.Queries;
using Domain.Features.Payrolls.Interfaces;
using ActivityProductivityWeightEntity = Domain.Features.Payrolls.Entities.ActivityProductivityWeight;

namespace Application.Features.Payrolls.Rules.ActivityProductivityWeight.Handlers
{
    public class GetActivityProductivityWeightsPagedHandler
    {
        private readonly IActivityProductivityWeightRepository _repository;
        private readonly ICacheService _cache;

        public GetActivityProductivityWeightsPagedHandler(
            IActivityProductivityWeightRepository repository,
            ICacheService cache)
        {
            _repository = repository;
            _cache = cache;
        }

        public async Task<PagedResponse<ActivityProductivityWeightResponse>> HandleAsync(
            GetActivityProductivityWeightsPagedQuery query)
        {
            string cacheKey = "activityProductivityWeights:all";
            var all = await _cache.GetAsync<List<ActivityProductivityWeightEntity>>(cacheKey);
            if (all == null)
            {
                all = (await _repository.GetAllAsync())?.ToList() ?? new List<ActivityProductivityWeightEntity>();
                await _cache.SetAsync(cacheKey, all, TimeSpan.FromMinutes(10));
            }

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