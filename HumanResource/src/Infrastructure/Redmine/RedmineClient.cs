using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;
using Application.Features.Redmine.Interfaces;
using Application.Features.Redmine.DTOs;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace Infrastructure.Redmine
{
    public class RedmineClient : IRedmineService
    {
        private readonly HttpClient _httpClient;
        private readonly IConfiguration _configuration;
        private readonly ILogger<RedmineClient> _logger;

        public RedmineClient(HttpClient httpClient, IConfiguration configuration, ILogger<RedmineClient> logger)
        {
            _httpClient = httpClient;
            _configuration = configuration;
            _logger = logger;
        }

        public async Task<List<RedmineUserDto>> GetUsersAsync()
        {
            const string endpoint = "/users.json";
            var response = await _httpClient.GetAsync(endpoint);
            await EnsureSuccessAndLogAsync(response, endpoint, null);

            var content = await response.Content.ReadAsStringAsync();
            var result = JsonSerializer.Deserialize<RedmineUsersResponse>(content);
            return result?.Users ?? new();
        }

        public async Task<List<RedmineProjectDto>> GetProjectsAsync()
        {
            var allProjects = new List<RedmineProjectDto>();
            
            try
            {
                // 1. Obtener proyectos activos (status=1)
                const string activeEndpoint = "/projects.json?status=1";
                var activeResponse = await _httpClient.GetAsync(activeEndpoint);
                await EnsureSuccessAndLogAsync(activeResponse, activeEndpoint, null);
                
                var activeContent = await activeResponse.Content.ReadAsStringAsync();
                var activeResult = JsonSerializer.Deserialize<RedmineProjectsResponse>(activeContent);
                var activeProjects = activeResult?.Projects ?? new List<RedmineProjectDto>();
                
                Console.WriteLine($"[REDMINE] Found {activeProjects.Count} active projects");
                allProjects.AddRange(activeProjects);
                
                // 2. Obtener proyectos cerrados (status=5)
                const string closedEndpoint = "/projects.json?status=5";
                var closedResponse = await _httpClient.GetAsync(closedEndpoint);
                await EnsureSuccessAndLogAsync(closedResponse, closedEndpoint, null);
                
                var closedContent = await closedResponse.Content.ReadAsStringAsync();
                var closedResult = JsonSerializer.Deserialize<RedmineProjectsResponse>(closedContent);
                var closedProjects = closedResult?.Projects ?? new List<RedmineProjectDto>();
                
                Console.WriteLine($"[REDMINE] Found {closedProjects.Count} closed projects");
                allProjects.AddRange(closedProjects);
                
                // 3. Obtener proyectos archivados (status=9)
                const string archivedEndpoint = "/projects.json?status=9";
                var archivedResponse = await _httpClient.GetAsync(archivedEndpoint);
                await EnsureSuccessAndLogAsync(archivedResponse, archivedEndpoint, null);
                
                var archivedContent = await archivedResponse.Content.ReadAsStringAsync();
                var archivedResult = JsonSerializer.Deserialize<RedmineProjectsResponse>(archivedContent);
                var archivedProjects = archivedResult?.Projects ?? new List<RedmineProjectDto>();
                
                Console.WriteLine($"[REDMINE] Found {archivedProjects.Count} archived projects");
                allProjects.AddRange(archivedProjects);
                
                // Eliminar duplicados por ID
                var uniqueProjects = allProjects
                    .GroupBy(p => p.Id)
                    .Select(g => g.First())
                    .ToList();
                
                Console.WriteLine($"[REDMINE] Total unique projects: {uniqueProjects.Count}");
                
                return uniqueProjects;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[REDMINE] Error getting all projects: {ex.Message}");
                // Fallback: intentar el método original
                const string fallbackEndpoint = "/projects.json";
                var fallbackResponse = await _httpClient.GetAsync(fallbackEndpoint);
                await EnsureSuccessAndLogAsync(fallbackResponse, fallbackEndpoint, null);

                var fallbackContent = await fallbackResponse.Content.ReadAsStringAsync();
                var fallbackResult = JsonSerializer.Deserialize<RedmineProjectsResponse>(fallbackContent);
                return fallbackResult?.Projects ?? new List<RedmineProjectDto>();
            }
        }

        public async Task<List<RedmineProjectDto>> GetAllProjectsAsync()
        {
            const string endpoint = "/projects.json";
            var response = await _httpClient.GetAsync(endpoint);
            await EnsureSuccessAndLogAsync(response, endpoint, null);

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
            await EnsureSuccessAndLogAsync(response, url, new { from, to, redmineUserId });

            var content = await response.Content.ReadAsStringAsync();
            var result = JsonSerializer.Deserialize<RedmineTimeEntriesResponse>(content);
            return result?.TimeEntries ?? new();
        }

        public async Task<List<RedmineMilestoneDto>> GetProjectMilestonesAsync(int projectId)
        {
            var url = $"/projects/{projectId}/versions.json";
            var response = await _httpClient.GetAsync(url);
            await EnsureSuccessAndLogAsync(response, url, new { projectId });

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

        public async Task<List<RedmineTimeEntryActivityDto>> GetTimeEntryActivitiesAsync()
        {
            const string endpoint = "/enumerations/time_entry_activities.json";
            var response = await _httpClient.GetAsync(endpoint);
            await EnsureSuccessAndLogAsync(response, endpoint, null);

            var content = await response.Content.ReadAsStringAsync();
            var result = JsonSerializer.Deserialize<RedmineTimeEntryActivitiesResponse>(content);
            return result?.TimeEntryActivities ?? new List<RedmineTimeEntryActivityDto>();
        }

        private async Task EnsureSuccessAndLogAsync(HttpResponseMessage response, string endpoint, object? context)
        {
            if (response.IsSuccessStatusCode)
                return;

            var statusCode = response.StatusCode;
            var body = await response.Content.ReadAsStringAsync();
            _logger.LogError(
                "Redmine API request failed. Endpoint={Endpoint}, StatusCode={StatusCode}, ResponseBody={ResponseBody}, Context={Context}",
                endpoint, (int)statusCode, body, context ?? "(none)");

            throw new HttpRequestException(
                $"Redmine API failed: {(int)statusCode} {statusCode}. Endpoint: {endpoint}. Response: {body}");
        }
    }
}