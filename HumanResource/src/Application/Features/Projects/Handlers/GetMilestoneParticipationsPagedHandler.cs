using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Application.Common;
using Application.Features.Projects.DTOs;
using Application.Features.Projects.Queries;
using Domain.Features.Projects.Interfaces;
using Application.Common.Interfaces;
using Domain.Features.Projects.Aggregates;

namespace Application.Features.Projects.Handlers
{
    public class GetMilestoneParticipationsPagedHandler
    {
        private readonly IMilestoneParticipationRepository _repository;
        private readonly ICacheService _cache;

        public GetMilestoneParticipationsPagedHandler(IMilestoneParticipationRepository repository, ICacheService cache)
        {
            _repository = repository;
            _cache = cache;
        }

        public async Task<PagedResponse<MilestoneParticipationResponse>> HandleAsync(GetMilestoneParticipationsPagedQuery query)
        {
            string cacheKey = "milestoneParticipations:all";
            var allItems = await _cache.GetAsync<List<MilestoneParticipation>>(cacheKey);
            if (allItems == null)
            {
                allItems = await _repository.GetAllAsync(); 
                await _cache.SetAsync(cacheKey, allItems, TimeSpan.FromMinutes(5)); 
            }

            var filtered = allItems.AsEnumerable();

            if (query.ProjectMilestoneId.HasValue)
                filtered = filtered.Where(x => x.ProjectMilestoneId == query.ProjectMilestoneId.Value);

            if (query.IsActive.HasValue)
                filtered = filtered.Where(x => x.IsActive == query.IsActive.Value);

            var filteredList = filtered.ToList();
            var totalItems = filteredList.Count;

            var paged = filteredList
                .Skip((query.Page - 1) * query.PageSize)
                .Take(query.PageSize)
                .Select(x => new MilestoneParticipationResponse
                {
                    Id = x.Id,
                    ProjectMilestoneId = x.ProjectMilestoneId,
                    EmployeeId = x.EmployeeId,
                    IsPaid = x.IsPaid,
                    IsActive = x.IsActive
                }).ToList();

            var totalPages = (int)Math.Ceiling(totalItems / (double)query.PageSize);

            return new PagedResponse<MilestoneParticipationResponse>
            {
                Items = paged,
                Page = query.Page,
                PageSize = query.PageSize,
                TotalItems = totalItems,
                TotalPages = totalPages
            };
        }
    }
}