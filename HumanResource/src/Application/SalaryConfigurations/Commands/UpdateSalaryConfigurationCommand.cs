using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Domain.Enums;

namespace Application.SalaryConfigurations.Commands
{
    public class UpdateSalaryConfigurationCommand
    {
        public EmployeeRole Role { get; set; }
        public decimal NewHourlyRate { get; set; }
    }
}