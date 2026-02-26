using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Domain.Enums;
using Domain.Common;
namespace Domain.Models
{
    public class Payroll : BaseEntity
    {
        public Guid Id { get; private set; }
        public Guid EmployeeId { get; private set; }

        public DateTime PeriodFrom { get; private set; }
        public DateTime PeriodTo { get; private set; }

        public decimal TotalHours { get; private set; }
        public decimal TotalAmount { get; private set; }

        public PayrollStatus Status { get; private set; }

        private readonly List<PayrollLine> _lines = new();
        public IReadOnlyCollection<PayrollLine> Lines => _lines;

        private readonly List<PayrollComponent> _components = new();
        public IReadOnlyCollection<PayrollComponent> Components => _components;

        protected Payroll() { }

        public Payroll(Guid employeeId, DateTime from, DateTime to)
        {
            if (from > to)
                throw new ArgumentException("Invalid payroll period.");
        
            Id = Guid.NewGuid();
            EmployeeId = employeeId;
            PeriodFrom = from;
            PeriodTo = to;
            Status = PayrollStatus.Draft;
        }

        private void RecalculateTotals()
        {
            TotalHours = _lines.Sum(l => l.Hours);

            TotalAmount =
                _lines.Sum(l => l.Amount)
                + _components.Sum(c => c.Amount);
        }

        public void AddLine(PayrollLine line)
        {
            if (Status == PayrollStatus.Closed)
                throw new InvalidOperationException("Cannot modify a closed payroll.");

            line.SetPayrollId(Id);
            _lines.Add(line);
            RecalculateTotals();
        }

        public void AddComponent(PayrollComponent component)
        {
            if (Status == PayrollStatus.Closed)
                throw new InvalidOperationException("Cannot modify a closed payroll.");

            component.SetPayrollId(Id);
            _components.Add(component);
            RecalculateTotals();
        }

        public void MarkUnderReview()
        {
            if (Status != PayrollStatus.Draft)
                throw new InvalidOperationException("Only draft payrolls can move to review.");

            Status = PayrollStatus.UnderReview;
            MarkUpdated();
        }

        public void Approve()
        {
            if (Status != PayrollStatus.UnderReview)
                throw new InvalidOperationException("Payroll must be under review to approve.");

            Status = PayrollStatus.Approved;
            MarkUpdated();
        }

        public void Close()
        {
            if (Status != PayrollStatus.Approved)
                throw new InvalidOperationException("Payroll must be approved before closing.");

            Status = PayrollStatus.Closed;
            MarkUpdated();
        }
    }
}