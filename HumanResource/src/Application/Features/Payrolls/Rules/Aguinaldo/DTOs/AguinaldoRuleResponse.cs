using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Application.Features.Payrolls.Rules.Aguinaldo.DTOs
{
    public class AguinaldoRuleResponse
    {
        public Guid Id { get; set; }
        public decimal MonthlyAccrualPercentage { get; set; }
        public int PayMonth { get; set; }
        public bool IsActive { get; set; }
    }
}