using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Text.Json.Serialization;

namespace Application.Redmine.DTOs
{
    public class RedmineTimeEntryDto
    {
        [JsonPropertyName("id")]
        public int Id { get; set; }

        [JsonPropertyName("hours")]
        public double Hours { get; set; }

        [JsonPropertyName("spent_on")]
        public DateTime SpentOn { get; set; }

        [JsonPropertyName("user")]
        public RedmineUserReference User { get; set; } = new();

        [JsonPropertyName("project")]
        public RedmineProjectReference Project { get; set; } = new();
    }

    public class RedmineUserReference
    {
        [JsonPropertyName("id")]
        public int Id { get; set; }
    }

    public class RedmineProjectReference
    {
        [JsonPropertyName("name")]
        public string Name { get; set; } = string.Empty;
        
        [JsonPropertyName("id")]
        public int Id { get; set; }
    }
}