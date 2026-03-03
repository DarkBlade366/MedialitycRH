using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Application.Features.Projects.DTOs;
using Application.Features.Projects.Queries;
using Domain.Features.Projects.Interfaces;

namespace Application.Features.Projects.Handlers
{
    public class GetMilestoneParticipationByIdHandler
    {
        private readonly IMilestoneParticipationRepository _repository;

        public GetMilestoneParticipationByIdHandler(IMilestoneParticipationRepository repository)
        {
            _repository = repository;
        }

        public async Task<MilestoneParticipationResponse?> HandleAsync(GetMilestoneParticipationByIdQuery query)
        {
            var participation = await _repository.GetByIdAsync(query.Id);
            
            if (participation == null)
                return null;

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