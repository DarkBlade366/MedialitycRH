using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Application.Features.Payrolls.Rules.BaseSalary.Commands
{
    public class ChangeBaseSalaryRuleStatusCommand
    {
        public Guid Id { get; set; }
        public bool IsActive { get; set; }
    }
}   