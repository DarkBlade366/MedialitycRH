using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Domain.Enums;
using Domain.Common;

namespace Domain.Models
{
    public class PayrollComponent : BaseEntity
    {
        public Guid Id { get; private set; }
        public Guid PayrollId { get; private set; }

        public PayrollComponentType Type { get; private set; }

        public string? Description { get; private set; }
        public decimal Amount { get; private set; }

        protected PayrollComponent() { }

        public PayrollComponent(PayrollComponentType type, string description, decimal amount)
        {
            Id = Guid.NewGuid();
            Type = type;
            Description = description;
            Amount = amount;
        }

        internal void SetPayrollId(Guid payrollId)
        {
            PayrollId = payrollId;
        }
    }
}