using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Application.Common.Interfaces;
using Application.Features.Projects.Commands;
using Application.Features.Projects.DTOs;
using Domain.Features.Employees.Interfaces;
using Domain.Features.Projects.Aggregates;
using Domain.Features.Projects.Interfaces;

namespace Application.Features.Projects.Handlers
{
    public class CreateMilestoneParticipationHandler
    {
        private readonly IMilestoneParticipationRepository _repositoryMilestoneParticipation;
        private readonly IEmployeeRepository _repositoryEmployee;
        private readonly IUnitOfWork _unitOfWork;

        public CreateMilestoneParticipationHandler(
            IMilestoneParticipationRepository repositoryMilestoneParticipation,
            IEmployeeRepository repositoryEmployee,
            IUnitOfWork unitOfWork)
        {
            _repositoryMilestoneParticipation = repositoryMilestoneParticipation;
            _repositoryEmployee = repositoryEmployee;
            _unitOfWork = unitOfWork;
        }

        public async Task<MilestoneParticipationResponse> HandleAsync(CreateMilestoneParticipationCommand command)
        {
            var milestone = await _repositoryMilestoneParticipation.GetMilestoneAsync(command.ProjectMilestoneId);

            if (milestone == null)
                throw new Exception("Project milestone not found.");

            var existingEmployee = await _repositoryEmployee.GetByIdAsync(command.EmployeeId);

            if (existingEmployee == null)
                throw new Exception ($"Employee {command.EmployeeId} does not exist.");

            var existing = await _repositoryMilestoneParticipation.GetByMilestoneAndEmployeeAsync(command.ProjectMilestoneId, command.EmployeeId);
            
            if (existing != null)
                throw new Exception("Participation already exists.");

            var participation = new MilestoneParticipation(command.ProjectMilestoneId, command.EmployeeId, milestone);

            await _repositoryMilestoneParticipation.AddAsync(participation);
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