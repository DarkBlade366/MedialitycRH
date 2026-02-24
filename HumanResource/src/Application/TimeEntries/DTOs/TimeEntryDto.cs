using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Application.TimeEntries.DTOs
{
    public class TimeEntryDto
    {
        public Guid Id { get; set; }
        public Guid EmployeeId { get; set; }
        public double Hours { get; set; }
        public DateTime SpentOn { get; set; }
        public string ProjectName { get; set; } = string.Empty;
    }
}