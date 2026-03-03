using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Application.Common.Interfaces;
using Application.Features.Projects.Commands;
using Application.Features.Projects.DTOs;
using Domain.Features.Projects.Aggregates;
using Domain.Features.Projects.Interfaces;

namespace Application.Features.Projects.Handlers
{
    public class CreateMilestoneParticipationHandler
    {
        private readonly IMilestoneParticipationRepository _repository;
        private readonly IUnitOfWork _unitOfWork;

        public CreateMilestoneParticipationHandler(
            IMilestoneParticipationRepository repository,
            IUnitOfWork unitOfWork)
        {
            _repository = repository;
            _unitOfWork = unitOfWork;
        }

        public async Task<MilestoneParticipationResponse> HandleAsync(CreateMilestoneParticipationCommand command)
        {
            var milestone = await _repository.GetMilestoneAsync(command.ProjectMilestoneId);

            if (milestone == null)
                throw new Exception("Project milestone not found.");

            var existing = await _repository.GetByMilestoneAndEmployeeAsync(command.ProjectMilestoneId, command.EmployeeId);
            
            if (existing != null)
                throw new Exception("Participation already exists.");

            var participation = new MilestoneParticipation(command.ProjectMilestoneId, command.EmployeeId, milestone);

            await _repository.AddAsync(participation);
            await _unitOfWork.SaveChangesAsync();

            return new MilestoneParticipationResponse
            {
                Id = participation.Id,
                ProjectMilestoneId = participation.ProjectMilestoneId,
                EmployeeId = participation.EmployeeId,
                IsPaid = participation.IsPaid,
                IsActive = true
            };
        }
    }
}