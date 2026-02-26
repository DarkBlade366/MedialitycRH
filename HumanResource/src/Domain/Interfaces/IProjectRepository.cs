using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Domain.Models;

namespace Domain.Interfaces
{
    public interface IProjectRepository
    {
        Task<Project?> GetByRedmineIdAsync(int redmineProjectId);
        Task<List<Project>> GetAllAsync();
        Task AddAsync(Project project);
        Task SaveChangesAsync();
    }
}