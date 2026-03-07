using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Application.Features.Payrolls.Rules.Project.DTOs
{
    public class ProjectRuleResponse
    {
        public Guid Id { get; set; }
        public int RedmineProjectId { get; set; }
        public decimal BonusAmount { get; set; }
        public bool IsActive { get; set; }
    }
}
