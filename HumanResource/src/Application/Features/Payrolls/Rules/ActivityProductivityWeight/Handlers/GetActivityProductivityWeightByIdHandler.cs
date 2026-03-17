using Application.Common.Interfaces;
using Application.Features.Payrolls.Rules.ActivityProductivityWeight.DTOs;
using Application.Features.Payrolls.Rules.ActivityProductivityWeight.Queries;
using Domain.Features.Payrolls.Interfaces;
using ActivityProductivityWeightEntity = Domain.Features.Payrolls.Entities.ActivityProductivityWeight;

namespace Application.Features.Payrolls.Rules.ActivityProductivityWeight.Handlers
{
    public class GetActivityProductivityWeightByIdHandler
    {
        private readonly IActivityProductivityWeightRepository _repository;
        private readonly ICacheService _cache;

        public GetActivityProductivityWeightByIdHandler(
            IActivityProductivityWeightRepository repository,
            ICacheService cache)
        {
            _repository = repository;
            _cache = cache;
        }

        public async Task<ActivityProductivityWeightResponse> HandleAsync(GetActivityProductivityWeightByIdQuery query)
        {
            string cacheKey = $"activityProductivityWeight:{query.Id}";
            var cached = await _cache.GetAsync<ActivityProductivityWeightResponse>(cacheKey);
            if (cached != null)
                return cached;

            var entity = await _repository.GetByIdAsync(query.Id);
            if (entity == null)
                throw new KeyNotFoundException($"ActivityProductivityWeight with Id {query.Id} not found.");

            var response = new ActivityProductivityWeightResponse
            {
                Id = entity.Id,
                RedmineActivityId = entity.RedmineActivityId,
                ActivityName = entity.ActivityName,
                Weight = entity.Weight,
                IsActive = entity.IsActive
            };

            await _cache.SetAsync(cacheKey, response, TimeSpan.FromMinutes(10));
            
            return response;
        }
    }
}