using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Application.Employees.Queries
{
    public class GetEmployeesQuery
    {
        public int Page { get; init; } = 1;
        public int PageSize { get; init; } = 10;
    }
}