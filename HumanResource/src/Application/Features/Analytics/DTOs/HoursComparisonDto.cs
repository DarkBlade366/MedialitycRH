using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Application.Features.Analytics.DTOs
{
    public class HoursComparisonDto
    {
        public Guid EmployeeId { get; set; }
        public string EmployeeName { get; set; } = string.Empty;
        public DateTime PeriodStart { get; set; }
        public DateTime PeriodEnd { get; set; }
        public decimal RegisteredHours { get; set; }      
        public decimal ExpectedHours { get; set; }        
        public decimal Difference { get; set; }
        public decimal Percentage { get; set; }
    }
}