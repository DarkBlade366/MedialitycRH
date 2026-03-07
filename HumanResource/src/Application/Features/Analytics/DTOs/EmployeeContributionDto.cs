using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Application.Features.Analytics.DTOs
{
    public class EmployeeContributionDto
    {
        public Guid EmployeeId { get; set; }
        public string EmployeeName { get; set; } = string.Empty;
        public decimal Hours { get; set; }
        public decimal Cost { get; set; }
    }
}