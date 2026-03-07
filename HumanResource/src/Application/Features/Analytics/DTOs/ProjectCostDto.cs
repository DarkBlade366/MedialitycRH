using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Application.Features.Analytics.DTOs
{
    public class ProjectCostDto
    {
        public int RedmineProjectId { get; set; }
        public string ProjectName { get; set; } = string.Empty;
        public decimal TotalHours { get; set; }
        public decimal EstimatedCost { get; set; }
        public List<EmployeeContributionDto> Contributions { get; set; } = new();
    }
}