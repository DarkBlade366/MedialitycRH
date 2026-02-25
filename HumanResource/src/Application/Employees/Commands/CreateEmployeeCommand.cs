using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Domain.Enums;

namespace Application.Employees.Commands
{
    public class CreateEmployeeCommand
    {
        public string FullName { get; init; } = string.Empty;
        public string Email { get; init; }  = string.Empty;
        public string Password { get; init; } = string.Empty;
        public int RedmineUserId { get; init; }
        public EmployeeRole Role { get; init; }
    }
}