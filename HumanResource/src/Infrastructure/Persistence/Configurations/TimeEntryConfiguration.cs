using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Domain.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Persistence.Configurations
{
    public class TimeEntryConfiguration : IEntityTypeConfiguration<TimeEntry>
    {
        public void Configure(EntityTypeBuilder<TimeEntry> builder)
        {
            builder.ToTable("time_entries");
            builder.HasKey(t => t.Id);
            builder.HasIndex(t => t.RedmineTimeEntryId).IsUnique();
            builder.Property(t => t.EmployeeId).IsRequired();
            builder.Property(t => t.Hours).IsRequired();
            builder.Property(t => t.SpentOn).IsRequired();
            builder.Property(t => t.ProjectName).IsRequired().HasMaxLength(200);
            builder.HasIndex(t => t.RedmineTimeEntryId).IsUnique();
        }
    }
}
