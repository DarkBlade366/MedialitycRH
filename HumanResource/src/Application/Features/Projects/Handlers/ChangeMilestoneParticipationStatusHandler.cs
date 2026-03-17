using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Application.Common.Interfaces;
using Application.Features.Projects.Commands;
using Domain.Features.Projects.Interfaces;

namespace Application.Features.Projects.Handlers
{
    public class ChangeMilestoneParticipationStatusHandler
    {
        private readonly IMilestoneParticipationRepository _repository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly ICacheService _cache;

        public ChangeMilestoneParticipationStatusHandler(IMilestoneParticipationRepository repository, IUnitOfWork unitOfWork, ICacheService cache)
        {
            _repository = repository;
            _unitOfWork = unitOfWork;
            _cache = cache;
        }

        public async Task HandleAsync(ChangeMilestoneParticipationStatusCommand command)
        {
            var participation = await _repository.GetByIdAsync(command.Id);
        
            if (participation == null)
                throw new Exception("Participation not found.");
        
            if (participation.IsPaid)
                throw new Exception("Cannot change status of a participation that has already been paid.");
        
            if (command.IsActive)
            {
                if (participation.IsActive)
                    throw new Exception("The participant is already active.");
                else
                    participation.Activate();
            }
            else
            {
                if (!participation.IsActive)
                    throw new Exception("The participant is already inactive.");
                else
                    participation.Deactivate();
            }
        
            _repository.Update(participation);
            await _unitOfWork.SaveChangesAsync();

            await _cache.RemoveAsync($"milestoneParticipation:{participation.Id}");
            await _cache.RemoveAsync("milestoneParticipations:all");
        }
    }
}