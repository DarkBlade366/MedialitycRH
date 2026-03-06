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
        public int? RedmineActivityId { get; private set; }
        public string? ActivityName { get; private set; }
        public Guid EmployeeId { get; private set; }

        public decimal Hours { get; private set; }
        public decimal? ApprovedHours { get; private set; }

        public bool Reviewed { get; private set; }
        public DateTime SpentOn { get; private set; }

        protected TimeEntry() { }

        public TimeEntry(
            int redmineTimeEntryId,
            int redmineProjectId,
            Guid employeeId,
            decimal hours,
            DateTime spentOn,
            int? redmineActivityId = null,
            string? activityName = null)
        {
            if (redmineTimeEntryId <= 0)
                throw new ArgumentException("RedmineTimeEntryId must be greater than zero.");

            if (redmineProjectId <= 0)
                throw new ArgumentException("RedmineProjectId must be greater than zero.");

            if (hours <= 0)
                throw new ArgumentException("Hours must be greater than zero.");

            Id = Guid.NewGuid();
            RedmineTimeEntryId = redmineTimeEntryId;
            RedmineProjectId = redmineProjectId;
            EmployeeId = employeeId;
            Hours = hours;
            SpentOn = spentOn;
            RedmineActivityId = redmineActivityId > 0 ? redmineActivityId : null;
            ActivityName = !string.IsNullOrWhiteSpace(activityName) ? activityName.Trim() : null;
            ApprovedHours = null;
            Reviewed = false;
        }

        public void Update(
            decimal hours,
            DateTime spentOn,
            int? activityId,
            string? activityName)
        {
            bool hoursChanged = Hours != hours;

            Hours = hours;
            SpentOn = spentOn;
            RedmineActivityId = activityId;
            ActivityName = activityName;

            if (hoursChanged)
            {
                Reviewed = false;
                ApprovedHours = null;
            }
        }

        public void Approve(decimal approvedHours)
        {
            if (approvedHours < 0 || approvedHours > Hours)
                throw new ArgumentException("Approved hours must be between 0 and registered hours.");

            ApprovedHours = approvedHours;
            Reviewed = true;
        }
    }
}