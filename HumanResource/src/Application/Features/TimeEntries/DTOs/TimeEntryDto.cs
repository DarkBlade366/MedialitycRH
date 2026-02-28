using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Application.Features.TimeEntries.DTOs
{
    public class TimeEntryDto
    {
        public Guid Id { get; set; }
        public Guid EmployeeId { get; set; }
        public decimal Hours { get; set; }
        public DateTime SpentOn { get; set; }
        public int ProjectId { get; set; }
    }
}
