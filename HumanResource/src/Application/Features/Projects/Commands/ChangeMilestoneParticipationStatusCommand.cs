using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Application.Features.Projects.Commands
{
    public class ChangeMilestoneParticipationStatusCommand
    {
        public Guid Id { get; set; }
        public bool IsActive { get; set; }
    }
}