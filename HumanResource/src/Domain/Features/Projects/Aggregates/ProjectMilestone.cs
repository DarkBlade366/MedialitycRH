using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Domain.Common;

namespace Domain.Features.Projects.Aggregates
{
    public class ProjectMilestone : BaseEntity
    {
        public Guid Id { get; private set; }
        public int RedmineProjectId { get; private set; }
        public string Name { get; private set; } = string.Empty;
        public DateTime? CompletedAt { get; private set; }
        public bool IsPaid { get; private set; }

        private ProjectMilestone() { }

        public ProjectMilestone(int redmineProjectId, string name)
        {
            if (string.IsNullOrWhiteSpace(name))
                throw new ArgumentException("Milestone name is required.");

            Id = Guid.NewGuid();
            RedmineProjectId = redmineProjectId;
            Name = name;
        }

        public void MarkAsCompleted(DateTime completedAt)
        {
            CompletedAt = completedAt;
            MarkUpdated();
        }

        public void MarkAsPaid()
        {
            IsPaid = true;
            MarkUpdated();
        }
    }
}