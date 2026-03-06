using Application.Features.Payrolls.Rules.ActivityProductivityWeight.DTOs;
using Application.Features.Payrolls.Rules.ActivityProductivityWeight.Queries;
using Domain.Features.Payrolls.Interfaces;

namespace Application.Features.Payrolls.Rules.ActivityProductivityWeight.Handlers
{
    public class GetActivityProductivityWeightByIdHandler
    {
        private readonly IActivityProductivityWeightRepository _repository;

        public GetActivityProductivityWeightByIdHandler(IActivityProductivityWeightRepository repository)
        {
            _repository = repository;
        }

        public async Task<ActivityProductivityWeightResponse> HandleAsync(GetActivityProductivityWeightByIdQuery query)
        {
            var entity = await _repository.GetByIdAsync(query.Id);
            if (entity == null)
                throw new KeyNotFoundException($"ActivityProductivityWeight with Id {query.Id} not found.");

            return new ActivityProductivityWeightResponse
            {
                Id = entity.Id,
                RedmineActivityId = entity.RedmineActivityId,
                ActivityName = entity.ActivityName,
                Weight = entity.Weight,
                IsActive = entity.IsActive
            };
        }
    }
}
