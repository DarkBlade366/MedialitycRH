using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Application.Features.Employees.Queries
{
    public class GetEmployeesQuery
    {
        public int Page { get; init; } 
        public int PageSize { get; init; }
    }
}
