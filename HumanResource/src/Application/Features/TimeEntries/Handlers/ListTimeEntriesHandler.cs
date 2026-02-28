using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Application.Features.TimeEntries.DTOs;
using Application.Features.TimeEntries.Queries;
using Domain.Features.TimeEntries.Interfaces;

namespace Application.Features.TimeEntries.Handlers
{
    public class ListTimeEntriesHandler
    {
        private readonly ITimeEntryRepository _repository;

        public ListTimeEntriesHandler(ITimeEntryRepository repository)
        {
            _repository = repository;
        }

        public async Task<List<TimeEntryDto>> Handle(ListTimeEntriesQuery query)
        {
            var fromUtc = DateTime.SpecifyKind(query.From, DateTimeKind.Utc);
            var toUtc = DateTime.SpecifyKind(query.To, DateTimeKind.Utc);

            var entries = await _repository.GetByEmployeeAndPeriodAsync(
                query.EmployeeId,
                fromUtc,
                toUtc
            );

            return entries
                .Select(e => new TimeEntryDto
                {
                    Id = e.Id,
                    EmployeeId = e.EmployeeId,
                    Hours = e.Hours,
                    SpentOn = e.SpentOn,
                    ProjectName = e.ProjectName,
                    ProjectId = e.RedmineProjectId
                }).ToList();
        }
    }
}
