using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Domain.Enums;

namespace Application.Employees
{
    public class CreateEmployeeCommand
    {
        public string FullName { get; init; } 
        public string Email { get; init; }
        public EmployeeRole Role { get; init; }
    }
}