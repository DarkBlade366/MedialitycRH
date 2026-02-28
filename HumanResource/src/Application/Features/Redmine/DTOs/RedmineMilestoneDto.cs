using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using Domain.Features.Projects.Enums;

namespace Application.Features.Redmine.DTOs
{
    public class RedmineMilestoneDto
    {
        [JsonPropertyName("project_id")]
        public int ProjectId { get; set; }

        [JsonPropertyName("name")]
        public string Name { get; set; } = string.Empty;
        [JsonPropertyName("status")]
        public string Status { get; set; } = "open";

        [JsonPropertyName("completed_on")]
        public DateTime? CompletedAt { get; set; }
    }
}