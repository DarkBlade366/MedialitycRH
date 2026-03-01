using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Domain.Features.Employees.Enums;

namespace Application.Features.Payrolls.Rules.BaseSalary.Queries
{
    public class GetBaseSalaryRulesPagedQuery
    {
        public int Page { get; set; }
        public int PageSize { get; set; }
        public bool? IsActive { get; set; }
        public EmployeeRole? Role { get; set; }
    }
}