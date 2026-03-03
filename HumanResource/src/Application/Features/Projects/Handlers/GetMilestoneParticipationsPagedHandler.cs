using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Application.Common;
using Application.Features.Projects.DTOs;
using Application.Features.Projects.Queries;
using Domain.Features.Projects.Interfaces;

namespace Application.Features.Projects.Handlers
{
    public class GetMilestoneParticipationsPagedHandler
    {
        private readonly IMilestoneParticipationRepository _repository;

        public GetMilestoneParticipationsPagedHandler(IMilestoneParticipationRepository repository)
        {
            _repository = repository;
        }

        public async Task<PagedResponse<MilestoneParticipationResponse>> HandleAsync(GetMilestoneParticipationsPagedQuery query)
        {
            var allItems = await _repository.GetAllAsync();

            if (query.ProjectMilestoneId.HasValue)
                allItems = allItems.Where(x => x.ProjectMilestoneId == query.ProjectMilestoneId.Value).ToList();

            if (query.IsActive.HasValue)
                allItems = allItems.Where(x => x.IsActive).ToList();

            var totalItems = allItems.Count;

            var paged = allItems
                .Skip((query.Page - 1) * query.PageSize)
                .Take(query.PageSize)
                .Select(x => new MilestoneParticipationResponse
                {
                    Id = x.Id,
                    ProjectMilestoneId = x.ProjectMilestoneId,
                    EmployeeId = x.EmployeeId,
                    IsPaid = x.IsPaid,
                    IsActive = true
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