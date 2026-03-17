using Application.Common.Interfaces;
using Application.Features.Payrolls.Rules.ActivityProductivityWeight.Commands;
using Domain.Features.Payrolls.Interfaces;

namespace Application.Features.Payrolls.Rules.ActivityProductivityWeight.Handlers
{
    public class ChangeActivityProductivityWeightStatusHandler
    {
        private readonly IActivityProductivityWeightRepository _repository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly ICacheService _cache;

        public ChangeActivityProductivityWeightStatusHandler(
            IActivityProductivityWeightRepository repository,
            IUnitOfWork unitOfWork,
            ICacheService cache)
        {
            _repository = repository;
            _unitOfWork = unitOfWork;
            _cache = cache;
        }

        public async Task HandleAsync(ChangeActivityProductivityWeightStatusCommand command)
        {
            var entity = await _repository.GetByIdAsync(command.Id);
            
            if (entity == null)
                throw new KeyNotFoundException($"ActivityProductivityWeight with Id {command.Id} not found.");

            if (command.IsActive)
                entity.Activate();
            else
                entity.Deactivate();

            _repository.Update(entity);
            await _unitOfWork.SaveChangesAsync();
            
            await _cache.RemoveAsync("activityProductivityWeights:all");
            await _cache.RemoveAsync($"activityProductivityWeight:{entity.Id}");
        }
    }
}