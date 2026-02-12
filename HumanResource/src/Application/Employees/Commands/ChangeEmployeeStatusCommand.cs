using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Application.Employees.Commands
{
    public class ChangeEmployeeStatusCommand
    {
        public Guid Id { get; init; }
        public bool IsActive { get; init; }
    }
}