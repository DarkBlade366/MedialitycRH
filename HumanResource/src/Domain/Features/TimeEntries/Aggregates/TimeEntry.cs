using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Domain.Common;

namespace Domain.Features.TimeEntries.Aggregates
{
    public class TimeEntry : BaseEntity
    {
        public Guid Id { get; private set; }
        public int RedmineTimeEntryId { get; private set; }
        public int RedmineProjectId { get; private set; }
        public Guid EmployeeId { get; private set; }
        public decimal Hours { get; private set; }
        public DateTime SpentOn { get; private set; }
        public string ProjectName { get; private set; } = string.Empty;

        protected TimeEntry() { }

        public TimeEntry(
            int redmineTimeEntryId,
            int redmineProjectId,
            Guid employeeId,
            decimal hours,
            DateTime spentOn,
            string projectName)
        {
            if (redmineTimeEntryId <= 0)
                throw new ArgumentException("RedmineTimeEntryId must be greater than zero.");

            if (redmineProjectId <= 0)
                throw new ArgumentException("RedmineProjectId must be greater than zero.");

            if (hours <= 0)
                throw new ArgumentException("Hours must be greater than zero.");

            if (string.IsNullOrWhiteSpace(projectName))
                throw new ArgumentException("Project name is required.");

            Id = Guid.NewGuid();
            RedmineTimeEntryId = redmineTimeEntryId;
            RedmineProjectId = redmineProjectId;
            EmployeeId = employeeId;
            Hours = hours;
            SpentOn = spentOn;
            ProjectName = projectName.Trim();
        }
    }
}