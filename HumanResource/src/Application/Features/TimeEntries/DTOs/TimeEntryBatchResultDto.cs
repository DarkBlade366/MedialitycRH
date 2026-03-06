using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Application.Features.TimeEntries.DTOs
{
    public class TimeEntryBatchResultDto
    {
        public Guid TimeEntryId { get; set; }
        public bool Success { get; set; }
        public string Message { get; set; } = string.Empty;
        public TimeEntryDto? TimeEntry { get; set; }
    }
}