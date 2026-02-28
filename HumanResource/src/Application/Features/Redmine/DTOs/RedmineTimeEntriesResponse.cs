using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Text.Json.Serialization;

namespace Application.Features.Redmine.DTOs
{
    public class RedmineTimeEntriesResponse
    {
        [JsonPropertyName("time_entries")]
        public List<RedmineTimeEntryDto> TimeEntries { get; set; } = new();
    }
}
