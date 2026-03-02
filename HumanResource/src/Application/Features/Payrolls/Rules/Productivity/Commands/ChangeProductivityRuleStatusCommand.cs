using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Application.Features.Payrolls.Rules.Productivity.Commands
{
    public class ChangeProductivityRuleStatusCommand
    {
        public Guid Id { get; set; }
        public bool IsActive { get; set; }
    }
}