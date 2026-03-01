using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Domain.Features.Employees.Enums;

namespace Application.Features.Payrolls.Rules.BaseSalary.DTOs
{
    public class BaseSalaryRuleResponse
    {
        public Guid Id { get; set; }
        public string Role { get; set; } = string.Empty;
        public decimal Amount { get; set; }
        public bool IsActive { get; set; }
    }
}