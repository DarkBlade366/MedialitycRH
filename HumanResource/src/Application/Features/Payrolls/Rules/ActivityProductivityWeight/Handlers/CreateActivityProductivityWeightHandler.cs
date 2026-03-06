using Application.Common.Interfaces;
using Application.Features.Payrolls.Rules.ActivityProductivityWeight.Commands;
using Application.Features.Payrolls.Rules.ActivityProductivityWeight.DTOs;
using Domain.Features.Payrolls.Interfaces;

namespace Application.Features.Payrolls.Rules.ActivityProductivityWeight.Handlers
{
    public class CreateActivityProductivityWeightHandler
    {
        private readonly IActivityProductivityWeightRepository _repository;
        private readonly IUnitOfWork _unitOfWork;

        public CreateActivityProductivityWeightHandler(
            IActivityProductivityWeightRepository repository,
            IUnitOfWork unitOfWork)
        {
            _repository = repository;
            _unitOfWork = unitOfWork;
        }

        public async Task<ActivityProductivityWeightResponse> HandleAsync(CreateActivityProductivityWeightCommand command)
        {
            var existing = await _repository.GetByRedmineActivityIdAsync(command.RedmineActivityId);
            if (existing != null)
                throw new InvalidOperationException(
                    $"An activity weight for RedmineActivityId {command.RedmineActivityId} ('{existing.ActivityName}') already exists.");

            var entity = new Domain.Features.Payrolls.Entities.ActivityProductivityWeight(
                command.RedmineActivityId,
                command.ActivityName,
                command.Weight);

            await _repository.AddAsync(entity);
            await _unitOfWork.SaveChangesAsync();

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
