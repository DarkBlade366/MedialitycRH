using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Domain.Common;

namespace Domain.Models
{
    public class PayrollLine : BaseEntity
    {
        public Guid Id { get; private set; }

        public Guid PayrollId { get; private set; }
        public Payroll? Payroll { get; private set; }

        public int ProjectId { get; private set; }
        public string? ProjectName { get; private set; }

        public decimal Hours { get; private set; }
        public decimal HourlyRate { get; private set; }
        public decimal Amount { get; private set; }

        private PayrollLine() { }

        public PayrollLine(int projectId, string projectName, decimal hours, decimal hourlyRate)
        {
            Id = Guid.NewGuid();
            ProjectId = projectId;
            ProjectName = projectName;
            Hours = hours;
            HourlyRate = hourlyRate;
            Amount = hours * hourlyRate;
        }
    }
}