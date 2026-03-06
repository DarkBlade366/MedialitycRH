using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Domain.Features.Payrolls.Entities;
using Domain.Features.Payrolls.Interfaces;
using Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Persistence.Repositories.Payrrolls
{
    public class ActivityProductivityWeightRepository : IActivityProductivityWeightRepository
    {
        private readonly ApiDbContext _context;

        public ActivityProductivityWeightRepository(ApiDbContext context)
        {
            _context = context;
        }

        public async Task<List<ActivityProductivityWeight>> GetAllAsync()
        {
            return await _context.ActivityProductivityWeights
                .AsNoTracking()
                .OrderBy(x => x.ActivityName)
                .ToListAsync();
        }

        public async Task<ActivityProductivityWeight?> GetByIdAsync(Guid id)
        {
            return await _context.ActivityProductivityWeights
                .FirstOrDefaultAsync(x => x.Id == id);
        }

        public async Task<ActivityProductivityWeight?> GetByRedmineActivityIdAsync(int redmineActivityId)
        {
            return await _context.ActivityProductivityWeights
                .FirstOrDefaultAsync(x => x.RedmineActivityId == redmineActivityId);
        }

        public async Task AddAsync(ActivityProductivityWeight entity)
        {
            await _context.ActivityProductivityWeights.AddAsync(entity);
        }

        public void Update(ActivityProductivityWeight entity)
        {
            _context.ActivityProductivityWeights.Update(entity);
        }
    }
}
