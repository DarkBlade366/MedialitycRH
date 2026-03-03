using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Domain.Features.Projects.Aggregates;

namespace Domain.Features.Projects.Interfaces
{
    public interface IMilestoneParticipationRepository
    {
        Task AddAsync(MilestoneParticipation participation);

        Task<List<MilestoneParticipation>> GetByEmployeeIdAsync(Guid employeeId);

        Task<bool> ExistsAsync(Guid projectMilestoneId, Guid employeeId);
        Task<ProjectMilestone?> GetMilestoneAsync(Guid milestoneId);
        Task<MilestoneParticipation?> GetByMilestoneAndEmployeeAsync(Guid milestoneId, Guid employeeId);
        Task<MilestoneParticipation?> GetByIdAsync(Guid id);
        Task<List<MilestoneParticipation>> GetAllAsync();
        void Update(MilestoneParticipation participation);
    }
}