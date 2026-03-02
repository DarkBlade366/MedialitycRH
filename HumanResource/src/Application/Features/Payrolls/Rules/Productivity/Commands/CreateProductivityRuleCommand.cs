using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Application.Features.Payrolls.Rules.Productivity.Commands
{
    public class CreateProductivityRuleCommand
    {
        public decimal MinimumTarget { get; set; }
        public decimal FullBonusTarget { get; set; }
        public decimal BonusValue { get; set; }
        public string BonusType { get; set; } = string.Empty;
        public decimal? MaxBonusCap { get; set; }
    }
}