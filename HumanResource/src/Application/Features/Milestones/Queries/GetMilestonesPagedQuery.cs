using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Domain.Features.Projects.Enums;

namespace Application.Features.Milestones.Queries
{
    public class GetMilestonesPagedQuery
    {
        public int Page { get; set; }
        public int PageSize { get; set; }
        public int? RedmineProjectId { get; set; }
        public string? Status { get; set; }
        public DateTime? From { get; set; }
        public DateTime? To { get; set; }
    }
}