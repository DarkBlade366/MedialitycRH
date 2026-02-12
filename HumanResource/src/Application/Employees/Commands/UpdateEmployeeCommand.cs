using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Domain.Enums;

namespace Application.Employees.Commands
{
    public class UpdateEmployeeCommand
    {
        public Guid Id { get; init; }
        public string FullName { get; init; } = string.Empty;
        public string Email { get; init; } = string.Empty;
        public EmployeeRole Role { get; init; }
    }
}