using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Domain.Enums;
using Domain.Common;

namespace Domain.Models
{
    public class SalaryConfiguration : BaseEntity
    {
        public Guid Id { get; private set; }
        public EmployeeRole Role { get; private set; }
        public decimal BaseHourlyRate { get; private set; }

        protected SalaryConfiguration() { }

        public SalaryConfiguration(EmployeeRole role, decimal baseHourlyRate)
        {
            Id = Guid.NewGuid();
            Role = role;
            BaseHourlyRate = baseHourlyRate;
        }

        public void UpdateBaseRate(decimal newRate)
        {
            BaseHourlyRate = newRate;
            MarkUpdated();
        }
    }
}