using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace Application.Features.Redmine.DTOs
{
    public class RedmineTimeEntryActivitiesResponse
    {
        [JsonPropertyName("time_entry_activities")]
        public List<RedmineTimeEntryActivityDto> TimeEntryActivities { get; set; } = new();
    }

    public class RedmineTimeEntryActivityDto
    {
        [JsonPropertyName("id")]
        public int Id { get; set; }

        [JsonPropertyName("name")]
        public string Name { get; set; } = string.Empty;

        [JsonPropertyName("is_default")]
        public bool IsDefault { get; set; }
    }
}
