using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Application.Features.Projects.Commands
{
    public class CreateMilestoneParticipationCommand
    {
        public Guid ProjectMilestoneId { get; set; }
        public Guid EmployeeId { get; set; }
    }
}