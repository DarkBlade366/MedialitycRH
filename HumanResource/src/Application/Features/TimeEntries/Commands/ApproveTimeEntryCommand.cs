using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Application.Features.TimeEntries.Commands
{
    public class ApproveTimeEntryCommand
    {
        public Guid TimeEntryId { get; set; }
        public decimal ApprovedHours { get; set; }
    }
}