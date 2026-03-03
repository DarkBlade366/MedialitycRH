using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Application.Features.Payrolls.Rules.Overtime.DTOs
{
    public class OvertimeRuleResponse
    {
        public Guid Id { get; set; }
        public int StandardHoursPerPeriod { get; set; }
        public decimal OvertimeMultiplier { get; set; }
        public decimal HourlyRate { get; set; }
        public bool IsActive { get; set; }
    }
}