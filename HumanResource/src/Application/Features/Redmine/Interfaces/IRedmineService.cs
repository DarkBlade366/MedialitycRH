using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Application.Features.Redmine.DTOs;

namespace Application.Features.Redmine.Interfaces
{
    public interface IRedmineService
    {
        Task<List<RedmineUserDto>> GetUsersAsync();

        Task<List<RedmineProjectDto>> GetProjectsAsync();

        Task<List<RedmineTimeEntryDto>> GetTimeEntriesAsync(DateTime from, DateTime to, int? redmineUserId = null);
        Task<List<RedmineProjectDto>> GetAllProjectsAsync();
        Task<List<RedmineMilestoneDto>> GetProjectMilestonesAsync(int projectId);
    }
}
