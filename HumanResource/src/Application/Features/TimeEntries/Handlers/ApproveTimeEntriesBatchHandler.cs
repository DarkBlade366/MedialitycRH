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
    public class ApproveTimeEntriesBatchHandler
    {
        private readonly ITimeEntryRepository _repository;
        private readonly IUnitOfWork _unitOfWork;

        public ApproveTimeEntriesBatchHandler(
            ITimeEntryRepository repository,
            IUnitOfWork unitOfWork)
        {
            _repository = repository;
            _unitOfWork = unitOfWork;
        }

        public async Task<List<TimeEntryBatchResultDto>> Handle(
            ApproveTimeEntriesBatchCommand command,
            CancellationToken ct)
        {
            var ids = command.Items.Select(x => x.TimeEntryId).ToList();
            var entries = await _repository.GetByIdsAsync(ids);

            var results = new List<TimeEntryBatchResultDto>();

            foreach (var item in command.Items)
            {
                var entry = entries.FirstOrDefault(x => x.Id == item.TimeEntryId);

                if (entry == null)
                {
                    results.Add(new TimeEntryBatchResultDto
                    {
                        TimeEntryId = item.TimeEntryId,
                        Success = false,
                        Message = "Time entry not found."
                    });
                    continue;
                }

                if (entry.Reviewed)
                {
                    results.Add(new TimeEntryBatchResultDto
                    {
                        TimeEntryId = entry.Id,
                        Success = false,
                        Message = "Time entry already reviewed."
                    });
                    continue;
                }

                entry.Approve(item.ApprovedHours);

                results.Add(new TimeEntryBatchResultDto
                {
                    TimeEntryId = entry.Id,
                    Success = true,
                    Message = $"Approved {entry.ApprovedHours}h of {entry.Hours}h",
                    TimeEntry = new TimeEntryDto
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
                    }
                });
            }

            await _unitOfWork.SaveChangesAsync(ct);

            return results;
        }
    }
}