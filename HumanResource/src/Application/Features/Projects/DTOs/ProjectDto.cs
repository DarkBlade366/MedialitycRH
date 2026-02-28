using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Application.Features.Projects.DTOs
{
    public class ProjectDto
    {
        public Guid Id { get; set; }
        public int RedmineProjectId { get; set; }
        public string Name { get; set; } = string.Empty;
    }
}