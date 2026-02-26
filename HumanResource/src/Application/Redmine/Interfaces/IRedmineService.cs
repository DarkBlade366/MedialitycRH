using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Application.Redmine.DTOs;

namespace Application.Redmine.Interfaces
{
    public interface IRedmineService
    {
        Task<List<RedmineUserDto>> GetUsersAsync();

        Task<List<RedmineProjectDto>> GetProjectsAsync();

        Task<List<RedmineTimeEntryDto>> GetTimeEntriesAsync(DateTime from, DateTime to, int? redmineUserId = null);
    }
}