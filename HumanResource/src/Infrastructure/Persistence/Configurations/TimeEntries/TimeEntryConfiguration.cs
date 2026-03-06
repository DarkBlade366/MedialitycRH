using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Domain.Features.TimeEntries.Aggregates;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Persistence.Configurations.TimeEntries
{
    public class TimeEntryConfiguration : IEntityTypeConfiguration<TimeEntry>
    {
        public void Configure(EntityTypeBuilder<TimeEntry> builder)
        {
            builder.ToTable("time_entries");
            builder.HasKey(t => t.Id);
            builder.Property(t => t.RedmineTimeEntryId).IsRequired();
            builder.HasIndex(t => t.RedmineTimeEntryId).IsUnique();
            builder.Property(t => t.RedmineProjectId).IsRequired();
            builder.Property(t => t.RedmineActivityId);
            builder.Property(t => t.ActivityName).HasMaxLength(100);
            builder.Property(t => t.EmployeeId).IsRequired();
            builder.Property(t => t.Hours).IsRequired().HasPrecision(18, 2);
            builder.Property(t => t.SpentOn).IsRequired();
        }
    }
}
