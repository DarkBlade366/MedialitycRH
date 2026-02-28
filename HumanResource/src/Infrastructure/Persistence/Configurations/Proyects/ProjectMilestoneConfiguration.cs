using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Domain.Features.Projects.Aggregates;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Persistence.Configurations.Proyects
{
    public class ProjectMilestoneConfiguration : IEntityTypeConfiguration<ProjectMilestone>
    {
        public void Configure(EntityTypeBuilder<ProjectMilestone> builder)
        {
            builder.ToTable("project_milestones");
            builder.HasKey(x => x.Id);    
            builder.Property(x => x.RedmineProjectId).IsRequired();
            builder.Property(x => x.Name).IsRequired().HasMaxLength(200);
            builder.Property(x => x.CompletedAt);
            builder.Property(x => x.IsPaid).IsRequired();
            builder.HasIndex(x => new { x.RedmineProjectId, x.Name })   .IsUnique();
        }
    }
}