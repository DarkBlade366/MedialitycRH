using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Application.Features.Payrolls.Rules.Aguinaldo.Commands
{
    public class CreateAguinaldoRuleCommand
    {
        public decimal MonthlyAccrualPercentage { get; set; }
        public int PayMonth { get; set; }
    }
}