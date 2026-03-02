using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Application.Features.Payrolls.Rules.Vacation.Commands
{
    public class ChangeVacationRuleStatusCommand
    {
        public Guid Id { get; set; }
        public bool IsActive { get; set; }
    }
}