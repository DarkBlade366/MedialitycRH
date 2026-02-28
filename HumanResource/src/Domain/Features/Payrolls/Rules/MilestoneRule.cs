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
            RedmineProjectId = projectId;
            MilestoneName = milestoneName;
            BonusAmount = bonusAmount;
        }
    }
}
