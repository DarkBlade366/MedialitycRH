using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Application.Common.Interfaces;
using Application.Features.TimeEntries.Commands;
using Application.Features.TimeEntries.DTOs;
using Domain.Features.TimeEntries.Interfaces;

namespace Application.Features.TimeEntries.Handlers
{
    public class ApproveTimeEntryHandler
    {
        private readonly ITimeEntryRepository _repository;
        private readonly IUnitOfWork _unitOfWork;

        public ApproveTimeEntryHandler(
            ITimeEntryRepository repository,
            IUnitOfWork unitOfWork)
        {
            _repository = repository;
            _unitOfWork = unitOfWork;
        }

        public async Task<TimeEntryDto> Handle(
            ApproveTimeEntryCommand command,
            CancellationToken ct)
        {
            var entry = await _repository.GetByIdAsync(command.TimeEntryId);

            if (entry == null)
                throw new Exception("Time entry not found.");

            if (entry.Reviewed)
                throw new InvalidOperationException("This time entry has already been approved.");

            entry.Approve(command.ApprovedHours);

            await _unitOfWork.SaveChangesAsync(ct);

            return new TimeEntryDto
            {
                Id = entry.Id,
                RedmineTimeEntryId = entry.RedmineTimeEntryId,
                RedmineProjectId = entry.RedmineProjectId,
                RedmineActivityId = entry.RedmineActivityId,
                ActivityName = entry.ActivityName,
                EmployeeId = entry.EmployeeId,
                Hours = entry.Hours,
                ApprovedHours = entry.ApprovedHours,
                Reviewed = entry.Reviewed,
                SpentOn = entry.SpentOn
            };
        }
    }
}