using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Domain.Features.Projects.Aggregates;

namespace Domain.Features.Projects.Interfaces
{
    public interface IProjectRepository
    {
        Task<Project?> GetByRedmineIdAsync(int redmineProjectId);
        Task<List<Project>> GetAllAsync();
        Task AddAsync(Project project);
        void Update(Project project);
        void Delete(Project project);
        Task<bool> ExistsAsync(int redmineProjectId);
    }
}
