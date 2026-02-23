using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Text.Json.Serialization;

namespace Application.Redmine.DTOs
{
    public class RedmineTimeEntriesResponse
    {
        [JsonPropertyName("time_entries")]
        public List<RedmineTimeEntryDto> TimeEntries { get; set; } = new();
    }
}