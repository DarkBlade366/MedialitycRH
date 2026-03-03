using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Domain.Features.Projects.Aggregates;
using Domain.Features.Projects.Interfaces;

namespace Infrastructure.Persistence.Repositories.Projects
{
    public class MilestoneParticipationRepository : IMilestoneParticipationRepository
    {
        private readonly ApiDbContext _context;

        public MilestoneParticipationRepository(ApiDbContext context)
        {
            _context = context;
        }

        public async Task AddAsync(MilestoneParticipation participation)
        {
            await _context.Set<MilestoneParticipation>().AddAsync(participation);
        }

        public async Task<List<MilestoneParticipation>> GetByEmployeeIdAsync(Guid employeeId)
        {
            return await _context.Set<MilestoneParticipation>()
                .Include((MilestoneParticipation mp) => mp.ProjectMilestone!)
                .Where(mp => mp.EmployeeId == employeeId)
                .ToListAsync();
        }

        public async Task<bool> ExistsAsync(Guid projectMilestoneId, Guid employeeId)
        {
            return await _context.Set<MilestoneParticipation>()
                .AnyAsync(mp =>
                    mp.ProjectMilestoneId == projectMilestoneId &&
                    mp.EmployeeId == employeeId);
        }

        public async Task<ProjectMilestone?> GetMilestoneAsync(Guid milestoneId)
        {
            return await _context.Set<ProjectMilestone>()
                .FirstOrDefaultAsync(x => x.Id == milestoneId);
        }

        public async Task<MilestoneParticipation?> GetByMilestoneAndEmployeeAsync(Guid milestoneId, Guid employeeId)
        {
            return await _context.Set<MilestoneParticipation>()
                .FirstOrDefaultAsync(x => x.ProjectMilestoneId == milestoneId && x.EmployeeId == employeeId);
        }

        public async Task<MilestoneParticipation?> GetByIdAsync(Guid id)
        {
            return await _context.Set<MilestoneParticipation>()
                .FirstOrDefaultAsync(x => x.Id == id);
        }

        public async Task<List<MilestoneParticipation>> GetAllAsync()
        {
            return await _context.Set<MilestoneParticipation>().ToListAsync();
        }

        public void Update(MilestoneParticipation participation)
        {
            _context.Set<MilestoneParticipation>().Update(participation);
        }
    }
}