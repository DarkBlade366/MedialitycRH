using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Application.Features.Payrolls.Rules.Milestones.Commands
{
    public class CreateMilestoneRuleCommand
    {
        public int RedmineProjectId { get; set; }
        public string MilestoneName { get; set; } = string.Empty;
        public decimal BonusAmount { get; set; }
    }
}