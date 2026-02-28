using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Application.Features.Redmine.DTOs
{
    public class RedmineMilestonesResponse
    {
        [JsonPropertyName("versions")]
        public List<RedmineMilestoneDto> Milestones { get; set; } = new();
    }
}