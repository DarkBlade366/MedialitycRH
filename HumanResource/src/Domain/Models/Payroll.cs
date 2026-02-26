using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Domain.Common;

namespace Domain.Models
{
    public class Payroll : BaseEntity
    {
        public Guid Id { get; private set; }

        public Guid EmployeeId { get; private set; }
        public Employee? Employee { get; private set; }

        public DateTime From { get; private set; }
        public DateTime To { get; private set; }

        public decimal TotalHours { get; private set; }
        public decimal TotalAmount { get; private set; }

        // No tiene pq coincidir con el CreatedAt, es la fecha de generación de la nómina
        public DateTime GeneratedAt { get; private set; }  

        private readonly List<PayrollLine> _lines = new();
        public IReadOnlyCollection<PayrollLine> Lines => _lines;

        private Payroll() { }

        public Payroll(Guid employeeId, DateTime from, DateTime to, decimal totalHours, decimal totalAmount)
        {
            Id = Guid.NewGuid();
            EmployeeId = employeeId;
            From = from;
            To = to;
            TotalHours = totalHours;
            TotalAmount = totalAmount;
            GeneratedAt = DateTime.UtcNow;
        }

        public void AddLine(PayrollLine line)
        {
            _lines.Add(line);
        }
    }
}