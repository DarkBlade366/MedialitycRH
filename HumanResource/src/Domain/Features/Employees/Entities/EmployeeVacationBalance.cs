using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Domain.Common;

namespace Domain.Features.Employees.Entities
{
    public class EmployeeVacationBalance : BaseEntity
    {
        public Guid Id { get; private set; }
        public Guid EmployeeId { get; private set; }

        public decimal AccruedDays { get; private set; }
        public decimal UsedDays { get; private set; }
        public decimal AvailableDays => AccruedDays - UsedDays;

        public DateTime? LastAccrualDate { get; private set; }

        private EmployeeVacationBalance() { }

        internal EmployeeVacationBalance(Guid employeeId)
        {
            Id = Guid.NewGuid();
            EmployeeId = employeeId;
            AccruedDays = 0;
            UsedDays = 0;
            LastAccrualDate = null;
        }

        internal void Accrue(decimal days)
        {
            if (days <= 0)
                throw new ArgumentException("Days must be positive.");

            AccruedDays += days;
            LastAccrualDate = DateTime.UtcNow;
        }

        internal void Use(decimal days)
        {
            if (days <= 0)
                throw new ArgumentException("Days must be positive.");

            if (days > AvailableDays)
                throw new InvalidOperationException("Not enough vacation balance.");

            UsedDays += days;
        }

        public bool HasAccruedThisMonth()
        {
            if (!LastAccrualDate.HasValue)
                return false;

            var now = DateTime.UtcNow;
            return LastAccrualDate.Value.Year == now.Year && LastAccrualDate.Value.Month == now.Month;
        }
        
        public void PayUsedDays(decimal daysToPay)
        {
            if (daysToPay <= 0)
                throw new ArgumentException("Days to pay must be greater than zero.");

            if (daysToPay > UsedDays)
                throw new InvalidOperationException("Cannot pay more days than used.");

            UsedDays -= daysToPay;
        }
    }
}
