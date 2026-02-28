using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Domain.Features.Projects.Enums;

namespace Application.Features.Milestones.DTOs
{
    public class ProjectMilestoneDto
    {
        public Guid Id { get; set; }
        public int RedmineProjectId { get; set; }
        public string Name { get; set; } = string.Empty;
        public DateTime? CompletedAt { get; set; }
        public string Status { get; set; } = string.Empty;
    }
}