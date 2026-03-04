using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Application.Services
{
    public class MonthlyPayrollExecutionResult
    {
        public int TotalEmployees { get; set; }
        public int CreatedPayrolls { get; set; }
        public int SkippedPayrolls { get; set; }
        public int FailedPayrolls { get; set; }
        public List<string> Errors { get; set; } = new();
    }
}