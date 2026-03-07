using System;

namespace Domain.Features.Payrolls.Rules
{
    public class ProjectRule : PayrollRule
    {
        public int RedmineProjectId { get; private set; }
        public decimal BonusAmount { get; private set; }

        private ProjectRule() : base("Project Rule") { }

        public ProjectRule(
            int redmineProjectId,
            decimal bonusAmount)
            : base("Project Rule")
        {
            if (redmineProjectId <= 0)
                throw new ArgumentException("RedmineProjectId must be greater than zero.");

            if (bonusAmount < 0)
                throw new ArgumentException("BonusAmount cannot be negative.");

            RedmineProjectId = redmineProjectId;
            BonusAmount = bonusAmount;
        }

        public void UpdateBonusAmount(decimal bonusAmount)
        {
            if (bonusAmount < 0)
                throw new ArgumentException("BonusAmount cannot be negative.");

            BonusAmount = bonusAmount;
        }
    }
}
