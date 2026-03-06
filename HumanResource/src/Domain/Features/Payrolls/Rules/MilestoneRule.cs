using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Domain.Features.Payrolls.Rules
{
    public class MilestoneRule : PayrollRule
    {
        public int RedmineProjectId { get; private set; }
        public string MilestoneName { get; private set; } = string.Empty;
        public decimal BonusAmount { get; private set; }

        private MilestoneRule() : base("Milestone Rule") { }

        public MilestoneRule(int projectId, string milestoneName, decimal bonusAmount)
            : base("Milestone Rule")
        {
            if (bonusAmount <= 0)
                throw new ArgumentException("Bonus must be greater than zero.");
            
            if (string.IsNullOrWhiteSpace(milestoneName))
                throw new ArgumentException("Milestone name required.");

            RedmineProjectId = projectId;
            MilestoneName = milestoneName;
            BonusAmount = bonusAmount;
        }
    }
}
