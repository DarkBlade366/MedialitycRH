using System;
using System.Linq;
using System.Threading.Tasks;
using System.Collections.Generic;
using Domain.Features.TimeEntries.Interfaces;
using Application.Common;
using Application.Features.TimeEntries.DTOs;
using Application.Features.TimeEntries.Queries;

namespace Application.Features.TimeEntries.Handlers
{
    public class ListPagedTimeEntriesHandler
    {
        private readonly ITimeEntryRepository _repository;

        public ListPagedTimeEntriesHandler(ITimeEntryRepository repository)
        {
            _repository = repository;
        }

        public async Task<PagedResponse<TimeEntryDto>> Handle(ListPagedTimeEntriesQuery query)
        {
            DateTime? fromUtc = null;
            DateTime? toUtc = null;

            if (query.From.HasValue)
                fromUtc = DateTime.SpecifyKind(query.From.Value, DateTimeKind.Utc);

            if (query.To.HasValue)
                toUtc = DateTime.SpecifyKind(query.To.Value, DateTimeKind.Utc);

            var (items, totalItems) = await _repository.GetPagedFilteredAsync(
                query.EmployeeId,
                fromUtc,
                toUtc,
                query.Page,
                query.PageSize
            );

            var dtos = items.Select(x => new TimeEntryDto
            {
                Id = x.Id,
                RedmineTimeEntryId = x.RedmineTimeEntryId,
                RedmineProjectId = x.RedmineProjectId,
                RedmineActivityId = x.RedmineActivityId,
                ActivityName = x.ActivityName,
                EmployeeId = x.EmployeeId,
                Hours = x.Hours,
                SpentOn = x.SpentOn
            }).ToList();

            var totalPages = (int)Math.Ceiling(totalItems / (double)query.PageSize);

            return new PagedResponse<TimeEntryDto>
            {
                Items = dtos,
                Page = query.Page,
                PageSize = query.PageSize,
                TotalItems = totalItems,
                TotalPages = totalPages
            };
        }
    }
}
