using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Application.Features.TimeEntries.Queries
{
    public class ListPagedTimeEntriesQuery
    {
        public Guid? EmployeeId { get; set; }
        public DateTime? From { get; set; }
        public DateTime? To { get; set; }

        public int Page { get; set; }
        public int PageSize { get; set; }
    }
}
