using System;
using System.Collections.Generic;
using System.Linq;
using Domain.Common;
using Domain.Features.Payrolls.Entities;
using Domain.Features.Payrolls.Enums;
using Domain.Features.Payrolls.Aggregates.Payments;

namespace Domain.Features.Payrolls.Aggregates
{
    public class Payroll : BaseEntity
    {
        public Guid Id { get; private set; }
        public Guid EmployeeId { get; private set; }
        public DateTime PeriodStart { get; private set; }
        public DateTime PeriodEnd { get; private set; }
        public PayrollStatus Status { get; private set; }

        private readonly List<PayrollComponent> _components = new();
        public IReadOnlyCollection<PayrollComponent> Components => _components.AsReadOnly();

        private readonly List<MilestonePayment> _milestonePayments = new();
        public IReadOnlyCollection<MilestonePayment> MilestonePayments => _milestonePayments.AsReadOnly();

        private readonly List<AguinaldoPayment> _aguinaldoPayments = new();
        public IReadOnlyCollection<AguinaldoPayment> AguinaldoPayments => _aguinaldoPayments.AsReadOnly();

        private readonly List<VacationPayment> _vacationPayments = new();
        public IReadOnlyCollection<VacationPayment> VacationPayments => _vacationPayments.AsReadOnly();

        private readonly List<ProductivityPayment> _productivityPayments = new();
        public IReadOnlyCollection<ProductivityPayment> ProductivityPayments => _productivityPayments.AsReadOnly();

        private readonly List<OvertimePayment> _overtimePayments = new();
        public IReadOnlyCollection<OvertimePayment> OvertimePayments => _overtimePayments.AsReadOnly();

        public decimal GrossAmount { get; private set; }
        public decimal TotalDeductions { get; private set; }
        public decimal NetAmount { get; private set; }

        private Payroll() { }

        public Payroll(Guid employeeId, DateTime periodStart, DateTime periodEnd)
        {
            if (periodEnd <= periodStart)
                throw new ArgumentException("Invalid payroll period.");

            Id = Guid.NewGuid();
            EmployeeId = employeeId;
            PeriodStart = periodStart;
            PeriodEnd = periodEnd;
            Status = PayrollStatus.Draft;
        }

        public void AddComponent(PayrollComponent component)
        {
            if (Status != PayrollStatus.Draft)
                throw new InvalidOperationException("Cannot modify payroll unless it is in Draft state.");

            _components.Add(component);
            MarkUpdated();
        }

        public void MarkAsCalculated()
        {
            if (!_components.Any(c => c.Category == PayrollComponentCategory.Earning))
                throw new InvalidOperationException("Payroll must contain at least one earning.");

            GrossAmount = _components
                .Where(c => c.Category == PayrollComponentCategory.Earning)
                .Sum(c => c.Amount);

            TotalDeductions = _components
                .Where(c => c.Category == PayrollComponentCategory.Deduction)
                .Sum(c => c.Amount);

            NetAmount = GrossAmount - TotalDeductions;
            Status = PayrollStatus.Calculated;

            MarkUpdated();
        }
        
        public void Approve()
        {
            if (Status != PayrollStatus.Calculated)
                throw new InvalidOperationException("Payroll must be calculated before approval.");

            Status = PayrollStatus.Approved;
            MarkUpdated();
        }

        public void MarkAsPaid()
        {
            if (Status != PayrollStatus.Approved)
                throw new InvalidOperationException("Payroll must be approved before payment.");

            Status = PayrollStatus.Paid;
            MarkUpdated();
        }

        public void AddMilestonePayment(Guid milestoneRuleId, decimal amount, DateTime paidAt)
        {
            if (Status != PayrollStatus.Draft && Status != PayrollStatus.Calculated)
                throw new InvalidOperationException("Cannot add milestone payment unless payroll is Draft or Calculated.");

            _milestonePayments.Add(new MilestonePayment(this.Id, milestoneRuleId, amount, paidAt));
            MarkUpdated();
        }

        public bool IsMilestonePaid(Guid milestoneRuleId)
        {
            return _milestonePayments.Any(p => p.MilestoneRuleId == milestoneRuleId);
        }

        public void AddAguinaldoPayment(Guid ruleId, decimal amount, DateTime paidAt)
        {
            _aguinaldoPayments.Add(new AguinaldoPayment(this.Id, ruleId, amount, paidAt));
            MarkUpdated();
        }

        public void AddVacationPayment(Guid ruleId, decimal amount, DateTime paidAt)
        {
            _vacationPayments.Add(new VacationPayment(this.Id, ruleId, amount, paidAt));
            MarkUpdated();
        }

        public void AddProductivityPayment(Guid ruleId, decimal amount, DateTime paidAt)
        {
            _productivityPayments.Add(new ProductivityPayment(this.Id, ruleId, amount, paidAt));
            MarkUpdated();
        }

        public void AddOvertimePayment(Guid ruleId, decimal amount, DateTime paidAt)
        {
            _overtimePayments.Add(new OvertimePayment(this.Id, ruleId, amount, paidAt));
            MarkUpdated();
        }
    }
}
