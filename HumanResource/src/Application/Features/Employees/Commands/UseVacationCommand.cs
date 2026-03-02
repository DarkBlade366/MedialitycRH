using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Application.Features.Employees.Commands
{
    public class UseVacationCommand
    {
        public Guid EmployeeId { get; set; }
        public decimal Days { get; set; }
    }
}