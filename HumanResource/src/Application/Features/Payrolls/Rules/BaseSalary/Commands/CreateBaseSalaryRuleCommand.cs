using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Domain.Features.Employees.Enums;

namespace Application.Features.Payrolls.Rules.BaseSalary.Commands
{
    public class CreateBaseSalaryRuleCommand
    {
        public string Role { get; set; } = string.Empty;
        public decimal Amount { get; set; }
    }
}