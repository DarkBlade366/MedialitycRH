using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Domain.Features.Payrolls.ValueObjects
{
    public class CompletedMilestone
    {
        public int ProjectId { get; }
        public string Name { get; }
        public DateTime CompletedAt { get; }

        public CompletedMilestone(int projectId, string name, DateTime completedAt)
        {
            if (string.IsNullOrWhiteSpace(name))
                throw new ArgumentException("Milestone name is required.");

            ProjectId = projectId;
            Name = name;
            CompletedAt = completedAt;
        }
    }
}
