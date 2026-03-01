using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Domain.Features.Employees.Enums;

namespace Domain.Features.Payrolls.Rules
{
    public class BaseSalaryRule : PayrollRule
    {
        public decimal Amount { get; private set; }
        public EmployeeRole Role { get; private set; }

        private BaseSalaryRule() : base("Base Salary Rule") { }

        public BaseSalaryRule(EmployeeRole role, decimal amount)
            : base($"Base Salary - {role}")
        {
            if (amount <= 0)
                throw new ArgumentException("Base salary must be greater than zero.");

            Role = role;
            Amount = amount;
        }
    }
}
