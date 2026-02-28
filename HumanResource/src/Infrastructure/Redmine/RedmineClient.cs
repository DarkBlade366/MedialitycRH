using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using Application.Features.Redmine.Interfaces;
using Application.Features.Redmine.DTOs;
using Microsoft.Extensions.Configuration;

namespace Infrastructure.Redmine
{
    public class RedmineClient : IRedmineService
    {
        private readonly HttpClient _httpClient;
        private readonly IConfiguration _configuration;

        public RedmineClient(HttpClient httpClient, IConfiguration configuration)
        {
            _httpClient = httpClient;
            _configuration = configuration;
        }

        public async Task<List<RedmineUserDto>> GetUsersAsync()
        {
            var response = await _httpClient.GetAsync("/users.json");
            response.EnsureSuccessStatusCode();

            var content = await response.Content.ReadAsStringAsync();
            var result = JsonSerializer.Deserialize<RedmineUsersResponse>(content);
            return result?.Users ?? new();
        }

        public async Task<List<RedmineProjectDto>> GetProjectsAsync()
        {
            var response = await _httpClient.GetAsync("/projects.json");
            response.EnsureSuccessStatusCode();

            var content = await response.Content.ReadAsStringAsync();
            var result = JsonSerializer.Deserialize<RedmineProjectsResponse>(content);
            return result?.Projects ?? new();
        }

        public async Task<List<RedmineTimeEntryDto>> GetTimeEntriesAsync(DateTime from, DateTime to, int? redmineUserId = null)
        {
            var url = $"/time_entries.json?from={from:yyyy-MM-dd}&to={to:yyyy-MM-dd}";

            if (redmineUserId.HasValue)
                url += $"&user_id={redmineUserId}";

            var response = await _httpClient.GetAsync(url);
            response.EnsureSuccessStatusCode();

            var content = await response.Content.ReadAsStringAsync();
            var result = JsonSerializer.Deserialize<RedmineTimeEntriesResponse>(content);
            return result?.TimeEntries ?? new();
        }

        public async Task<List<RedmineProjectDto>> GetAllProjectsAsync()
        {
            var response = await _httpClient.GetAsync("/projects.json");
            response.EnsureSuccessStatusCode();

            var content = await response.Content.ReadAsStringAsync();
            var result = JsonSerializer.Deserialize<RedmineProjectsResponse>(content);
            return result?.Projects ?? new();
        }

        public async Task<List<RedmineMilestoneDto>> GetProjectMilestonesAsync(int projectId)
        {
            var url = $"/projects/{projectId}/versions.json";
            var response = await _httpClient.GetAsync(url);
            response.EnsureSuccessStatusCode();

            var content = await response.Content.ReadAsStringAsync();
            var result = JsonSerializer.Deserialize<RedmineMilestonesResponse>(content);
            
            return result?.Milestones
                .Select(m => new RedmineMilestoneDto
                {
                    ProjectId = projectId,
                    Name = m.Name,
                    Status = m.Status,   
                    CompletedAt = m.CompletedAt
                })
                .ToList() ?? new List<RedmineMilestoneDto>();
        }
    }
}