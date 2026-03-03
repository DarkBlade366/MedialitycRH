using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Application.Features.Projects.Queries
{
    public class GetMilestoneParticipationsPagedQuery
    {
        public int Page { get; set; }
        public int PageSize { get; set; }
        public bool? IsActive { get; set; }
        public Guid? ProjectMilestoneId { get; set; }
    }
}