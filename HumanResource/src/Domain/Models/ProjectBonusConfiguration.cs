using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Domain.Common;

namespace Domain.Models
{
    public class ProjectBonusConfiguration : BaseEntity
    {
        public Guid Id { get; private set; }
        public int RedmineProjectId { get; private set; }
        public decimal ExtraHourlyRate { get; private set; }

        protected ProjectBonusConfiguration() { }

        public ProjectBonusConfiguration(int redmineProjectId, decimal extraHourlyRate)
        {
            Id = Guid.NewGuid();
            RedmineProjectId = redmineProjectId;
            ExtraHourlyRate = extraHourlyRate;
        }

        public void UpdateExtraRate(decimal newRate)
        {
            ExtraHourlyRate = newRate;
            MarkUpdated();
        }
    }
}