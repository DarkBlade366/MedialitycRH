using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Application.Features.Projects.DTOs;
using Application.Features.Projects.Queries;
using Domain.Features.Projects.Interfaces;
using Application.Common.Interfaces;

namespace Application.Features.Projects.Handlers
{
    public class GetMilestoneParticipationByIdHandler
    {
        private readonly IMilestoneParticipationRepository _repository;
        private readonly ICacheService _cache;

        public GetMilestoneParticipationByIdHandler(IMilestoneParticipationRepository repository, ICacheService cache)
        {
            _repository = repository;
            _cache = cache;
        }

        public async Task<MilestoneParticipationResponse?> HandleAsync(GetMilestoneParticipationByIdQuery query)
        {
            string cacheKey = $"milestoneParticipation:{query.Id}";
            var cached = await _cache.GetAsync<MilestoneParticipationResponse>(cacheKey);
            if (cached != null)
                return cached;

            var participation = await _repository.GetByIdAsync(query.Id);
            
            if (participation == null)
                return null;

            var response = new MilestoneParticipationResponse
            {
                Id = participation.Id,
                ProjectMilestoneId = participation.ProjectMilestoneId,
                EmployeeId = participation.EmployeeId,
                IsPaid = participation.IsPaid,
                IsActive = true
            };

            await _cache.SetAsync(cacheKey, response, TimeSpan.FromMinutes(5));

            return response;
        }
    }
}