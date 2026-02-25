using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Domain.Common;

namespace Domain.Models
{
    public class TimeEntry : BaseEntity
    {
        public Guid Id { get; private set; }
        public int RedmineTimeEntryId { get; private set; }
        public int RedmineProjectId { get; private set; }
        public Guid EmployeeId { get; private set; }
        public double Hours { get; private set; }
        public DateTime SpentOn { get; private set; }
        public string ProjectName { get; private set; } = string.Empty;

        protected TimeEntry() { }

        public TimeEntry(int redmineTimeEntryId, int redmineProjectId, Guid employeeId, double hours, DateTime spentOn, string projectName)
        {
            Id = Guid.NewGuid();
            RedmineTimeEntryId = redmineTimeEntryId;
            RedmineProjectId = redmineProjectId;
            EmployeeId = employeeId;
            Hours = hours;
            SpentOn = spentOn;
            ProjectName = projectName;
        }
    }
}