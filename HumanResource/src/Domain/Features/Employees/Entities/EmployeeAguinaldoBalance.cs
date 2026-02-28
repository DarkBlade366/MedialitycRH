using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Domain.Common;

namespace Domain.Features.Employees.Entities
{
    public class EmployeeAguinaldoBalance : BaseEntity
    {
        public Guid Id { get; private set; }
        public Guid EmployeeId { get; private set; }

        public decimal AccruedAmount { get; private set; }
        public decimal PaidAmount { get; private set; }

        private EmployeeAguinaldoBalance() { }

        internal EmployeeAguinaldoBalance(Guid employeeId)
        {
            Id = Guid.NewGuid();
            EmployeeId = employeeId;
            AccruedAmount = 0;
            PaidAmount = 0;
        }

        internal void Accrue(decimal amount)
        {
            if (amount <= 0)
                throw new ArgumentException("Amount must be positive.");

            AccruedAmount += amount;
            MarkUpdated();
        }

        internal decimal Pay()
        {
            if (AccruedAmount <= 0)
                throw new InvalidOperationException("No aguinaldo to pay.");

            var total = AccruedAmount;
            PaidAmount += total;
            AccruedAmount = 0;

            MarkUpdated();
            return total;
        }
    }
}
