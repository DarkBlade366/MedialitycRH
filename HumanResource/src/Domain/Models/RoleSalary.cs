using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Domain.Common;
using Domain.Enums;

namespace Domain.Models
{
    public class RoleSalary : BaseEntity
    {
        public Guid Id { get; private set; }

        public EmployeeRole Role { get; private set; }
        public decimal BaseHourlyRate { get; private set; }

        private RoleSalary() { }

        public RoleSalary(EmployeeRole role, decimal baseHourlyRate)
        {
            Id = Guid.NewGuid();
            Role = role;
            BaseHourlyRate = baseHourlyRate;
        }

        public void UpdateRate(decimal newRate)
        {
            BaseHourlyRate = newRate;
        }
    }
}