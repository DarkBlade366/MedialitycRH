using Application.Common.Interfaces;
using Application.Features.Payrolls.Rules.ActivityProductivityWeight.Commands;
using Application.Features.Payrolls.Rules.ActivityProductivityWeight.DTOs;
using Domain.Features.Payrolls.Interfaces;
using ActivityProductivityWeightEntity = Domain.Features.Payrolls.Entities.ActivityProductivityWeight;

namespace Application.Features.Payrolls.Rules.ActivityProductivityWeight.Handlers
{
    public class CreateActivityProductivityWeightHandler
    {
        private readonly IActivityProductivityWeightRepository _repository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly ICacheService _cache;

        public CreateActivityProductivityWeightHandler(
            IActivityProductivityWeightRepository repository,
            IUnitOfWork unitOfWork,
            ICacheService cache)
        {
            _repository = repository;
            _unitOfWork = unitOfWork;
            _cache = cache;
        }

        public async Task<ActivityProductivityWeightResponse> HandleAsync(CreateActivityProductivityWeightCommand command)
        {
            var existing = await _repository.GetByRedmineActivityIdAsync(command.RedmineActivityId);
            if (existing != null)
                throw new InvalidOperationException(
                    $"An activity weight for RedmineActivityId {command.RedmineActivityId} ('{existing.ActivityName}') already exists.");

            var entity = new ActivityProductivityWeightEntity(
                command.RedmineActivityId,
                command.ActivityName,
                command.Weight);

            await _repository.AddAsync(entity);
            await _unitOfWork.SaveChangesAsync();

            await _cache.RemoveAsync("activityProductivityWeights:all");

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