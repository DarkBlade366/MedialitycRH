using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Application.Features.Payrolls.Rules.Project.Commands
{
    public class CreateProjectRuleCommand
    {
        public int RedmineProjectId { get; set; }
        public decimal BonusAmount { get; set; }
    }
}
