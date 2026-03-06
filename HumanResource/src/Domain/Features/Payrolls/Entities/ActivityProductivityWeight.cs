using System;
using Domain.Common;

namespace Domain.Features.Payrolls.Entities
{
    public class ActivityProductivityWeight : BaseEntity
    {
        public Guid Id { get; private set; }
        public int RedmineActivityId { get; private set; }
        public string ActivityName { get; private set; } = string.Empty;
        public decimal Weight { get; private set; }
        public bool IsActive { get; private set; }

        private ActivityProductivityWeight() { }

        public ActivityProductivityWeight(int redmineActivityId, string activityName, decimal weight)
        {
            if (redmineActivityId <= 0)
                throw new ArgumentException("RedmineActivityId must be greater than zero.");

            if (string.IsNullOrWhiteSpace(activityName))
                throw new ArgumentException("ActivityName cannot be empty.");

            if (weight < 0 || weight > 1)
                throw new ArgumentException("Weight must be between 0 and 1.");

            Id = Guid.NewGuid();
            RedmineActivityId = redmineActivityId;
            ActivityName = activityName.Trim();
            Weight = weight;
            IsActive = true;
        }

        public void UpdateWeight(decimal weight)
        {
            if (weight < 0 || weight > 1)
                throw new ArgumentException("Weight must be between 0 and 1.");

            Weight = weight;
            MarkUpdated();
        }

        public void Deactivate()
        {
            IsActive = false;
            MarkUpdated();
        }

        public void Activate()
        {
            IsActive = true;
            MarkUpdated();
        }
    }
}
