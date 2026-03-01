using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Application.Features.Projects.Queries
{
    public class GetProjectsPagedQuery
    {
        public int Page { get; set; }
        public int PageSize { get; set; }
    }
}