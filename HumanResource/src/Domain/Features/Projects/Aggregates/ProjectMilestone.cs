using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Domain.Common;
using Domain.Features.Projects.Enums;

namespace Domain.Features.Projects.Aggregates
{
    public class ProjectMilestone : BaseEntity
    {
        public Guid Id { get; private set; }
        public int RedmineProjectId { get; private set; }
        public string Name { get; private set; } = string.Empty;
        public DateTime? CompletedAt { get; private set; }
        public MilestoneStatus Status { get; private set; }

        public ICollection<MilestoneParticipation> Participations { get; private set; } = new List<MilestoneParticipation>();

        private ProjectMilestone() { }

        public ProjectMilestone(int redmineProjectId, string name)
        {
            if (string.IsNullOrWhiteSpace(name))
                throw new ArgumentException("Milestone name is required.");

            Id = Guid.NewGuid();
            RedmineProjectId = redmineProjectId;
            Name = name;
            Status = MilestoneStatus.Pending;
        }

        public void MarkAsCompleted(DateTime completedAt)
        {
            CompletedAt = completedAt.ToUniversalTime();
            Status = MilestoneStatus.Completed;
        }

        public void MarkAsCancelled()
        {
            Status = MilestoneStatus.Cancelled;
        }

        public bool IsPending() => Status == MilestoneStatus.Pending;
        public bool IsCompleted() => Status == MilestoneStatus.Completed;
        public bool IsCancelled() => Status == MilestoneStatus.Cancelled;

        public void Reopen()
        {
            Status = MilestoneStatus.Pending;
            CompletedAt = null;
        }
    }
}