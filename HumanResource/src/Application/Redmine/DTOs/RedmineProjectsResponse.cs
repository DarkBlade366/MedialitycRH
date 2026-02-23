using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Text.Json.Serialization;

namespace Application.Redmine.DTOs
{
    public class RedmineProjectsResponse
    {
        [JsonPropertyName("projects")]
        public List<RedmineProjectDto> Projects { get; set; } = new();
    }
}