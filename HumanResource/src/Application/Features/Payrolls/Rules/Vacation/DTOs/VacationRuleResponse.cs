using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Application.Features.Payrolls.Rules.Vacation.DTOs
{
    public class VacationRuleResponse
    {
        public Guid Id { get; set; }
        public decimal AccrualRatePerMonth { get; set; }
        public bool IsActive { get; set; }
    }
}