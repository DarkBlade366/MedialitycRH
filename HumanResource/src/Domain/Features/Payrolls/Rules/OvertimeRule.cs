using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Domain.Features.Payrolls.Rules
{
    public class OvertimeRule : PayrollRule
    {
        public int StandardHoursPerPeriod { get; private set; }
        public decimal OvertimeMultiplier { get; private set; }

        private OvertimeRule() : base("Overtime Rule") { }

        public OvertimeRule(int standardHoursPerPeriod, decimal overtimeMultiplier)
            : base("Overtime Rule")
        {
            if (standardHoursPerPeriod <= 0)
                throw new ArgumentException("Standard hours must be greater than zero.");

            if (overtimeMultiplier <= 1)
                throw new ArgumentException("Overtime multiplier must be greater than 1.");

            StandardHoursPerPeriod = standardHoursPerPeriod;
            OvertimeMultiplier = overtimeMultiplier;
        }
    }
}
