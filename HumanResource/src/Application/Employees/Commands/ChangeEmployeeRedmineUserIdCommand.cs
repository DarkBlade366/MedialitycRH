using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Application.Employees.Commands
{
    public class ChangeEmployeeRedmineUserIdCommand
    {
        public Guid Id { get; init; }
        public int RedmineUserId { get; init; }
    }
}