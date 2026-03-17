using System;
using System.Linq;
using System.Threading.Tasks;
using System.Collections.Generic;
using Domain.Features.TimeEntries.Interfaces;
using Application.Common;
using Application.Features.TimeEntries.DTOs;
using Application.Features.TimeEntries.Queries;
using Application.Common.Interfaces;

namespace Application.Features.TimeEntries.Handlers
{
    public class ListPagedTimeEntriesHandler
    {
        private readonly ITimeEntryRepository _repository;
        private readonly ICacheService _cache;

        public ListPagedTimeEntriesHandler(ITimeEntryRepository repository, ICacheService cache)
        {
            _repository = repository;
            _cache = cache;
        }

        public async Task<PagedResponse<TimeEntryDto>> Handle(ListPagedTimeEntriesQuery query)
        {
            string cacheKey = $"timeentries:paged:{query.EmployeeId}:{query.From}:{query.To}:{query.Page}:{query.PageSize}";
            var cached = await _cache.GetAsync<PagedResponse<TimeEntryDto>>(cacheKey);
            if (cached != null) 
                return cached;

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

            var response = new PagedResponse<TimeEntryDto>
            {
                Items = dtos,
                Page = query.Page,
                PageSize = query.PageSize,
                TotalItems = totalItems,
                TotalPages = totalPages
            };

            await _cache.SetAsync(cacheKey, response, TimeSpan.FromMinutes(5));

            return response;
        }
    }
}
