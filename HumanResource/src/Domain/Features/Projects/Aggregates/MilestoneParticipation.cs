using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Domain.Common;

namespace Domain.Features.Projects.Aggregates
{
    public class MilestoneParticipation : BaseEntity
    {
        public Guid Id { get; private set; }

        public Guid ProjectMilestoneId { get; private set; }
        public Guid EmployeeId { get; private set; }

        public bool IsPaid { get; private set; }

        public ProjectMilestone? ProjectMilestone { get; private set; }
        public bool IsActive { get; set; }

        private MilestoneParticipation() { }

        public MilestoneParticipation(
            Guid milestoneId,
            Guid employeeId,
            ProjectMilestone projectMilestone)
        {
            Id = Guid.NewGuid();
            ProjectMilestoneId = milestoneId;
            EmployeeId = employeeId;
            ProjectMilestone = projectMilestone ?? throw new ArgumentNullException(nameof(projectMilestone));
            IsPaid = false;
            IsActive = true;
        }

        public void MarkAsPaid()
        {
            IsPaid = true;
        }

        public void Deactivate()
        {
            IsActive = false;
        }

        public void Activate()
        {
            IsActive = true;
        }
    }
}
