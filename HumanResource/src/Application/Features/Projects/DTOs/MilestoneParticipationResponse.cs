using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Application.Features.Projects.DTOs
{
    public class MilestoneParticipationResponse
    {
        public Guid Id { get; set; }
        public Guid ProjectMilestoneId { get; set; }
        public Guid EmployeeId { get; set; }
        public bool IsPaid { get; set; }
        public bool IsActive { get; set; }
    }
}