using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Domain.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Persistence.Configurations
{
    public class ProjectBonusConfigurationEntity : IEntityTypeConfiguration<ProjectBonusConfiguration>
    {
        public void Configure(EntityTypeBuilder<ProjectBonusConfiguration> builder)
        {
            builder.ToTable("project_bonus_configurations");
            builder.HasKey(p => p.Id);
            builder.Property(p => p.ExtraHourlyRate).HasColumnType("decimal(10,2)");
            builder.HasIndex(p => p.RedmineProjectId).IsUnique();
        }
    }
}