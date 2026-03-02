using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Application.Features.Payrolls.Rules.Overtime.Commands
{
    public class ChangeOvertimeRuleStatusCommand
    {
        public Guid Id { get; set; }
        public bool IsActive { get; set; }
    }
}