using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Domain.Features.Payrolls.Enums;

namespace Application.Features.Payrolls.Rules.Deduction.DTOs
{
    public class DeductionRuleResponse
    {
        public Guid Id { get; set; }
        public string Description { get; set; } = string.Empty;
        public decimal Percentage { get; set; }
        public string Type { get; set; } = string.Empty;
        public bool IsActive { get; set; }
    }
}