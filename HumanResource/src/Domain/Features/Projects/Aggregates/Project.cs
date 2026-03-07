using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Domain.Features.Projects.Enums;

namespace Domain.Features.Projects.Aggregates
{
    public class Project
    {
        public Guid Id { get; private set; }
        public int RedmineProjectId { get; private set; }
        public string? Name { get; private set; }
        public ProjectStatus Status { get; private set; }
        public DateTime? CompletedAt { get; private set; }

        private Project() { }

        public Project(int redmineProjectId, string name) : this(redmineProjectId, name, ProjectStatus.Active) { }

        public Project(int redmineProjectId, string name, ProjectStatus status)
        {
            if (redmineProjectId <= 0)
                throw new ArgumentException("RedmineProjectId must be greater than zero.");

            if (string.IsNullOrWhiteSpace(name))
                throw new ArgumentException("Project name is required.");

            Id = Guid.NewGuid();
            RedmineProjectId = redmineProjectId;
            Name = name;
            Status = status;
        }

        public void UpdateName(string name)
        {
            if (string.IsNullOrWhiteSpace(name))
                throw new ArgumentException("Project name is required.");

            Name = name;
        }
        public void UpdateStatus(ProjectStatus status)
        {
            Status = status;
        
            if (status == ProjectStatus.Completed)
                CompletedAt = DateTime.UtcNow;
        }
    }
}
