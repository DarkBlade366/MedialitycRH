using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Text.Json.Serialization;

namespace Application.Redmine.DTOs
{
    public class RedmineUserDto
    {
        [JsonPropertyName("id")]
        public int Id { get; set; }

        [JsonPropertyName("firstname")]
        public string FirstName { get; set; } = string.Empty;

        [JsonPropertyName("lastname")]
        public string LastName { get; set; } = string.Empty;

        [JsonPropertyName("mail")]
        public string Email { get; set; } = string.Empty;
    }
}