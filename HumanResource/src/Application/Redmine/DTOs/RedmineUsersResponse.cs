using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Text.Json.Serialization;

namespace Application.Redmine.DTOs
{
    public class RedmineUsersResponse
    {
        [JsonPropertyName("users")]
        public List<RedmineUserDto> Users { get; set; } = new();
    }
}