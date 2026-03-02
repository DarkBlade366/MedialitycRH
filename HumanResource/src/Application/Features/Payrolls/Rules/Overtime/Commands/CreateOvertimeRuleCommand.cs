using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Application.Features.Payrolls.Rules.Overtime.Commands
{
    public class CreateOvertimeRuleCommand
    {
        public int StandardHoursPerPeriod { get; set; }
        public decimal OvertimeMultiplier { get; set; }
    }
}