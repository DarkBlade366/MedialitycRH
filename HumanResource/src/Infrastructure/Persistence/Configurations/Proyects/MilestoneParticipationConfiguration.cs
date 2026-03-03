using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Domain.Features.Projects.Aggregates;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Persistence.Configurations.Proyects
{
    public class MilestoneParticipationConfiguration : IEntityTypeConfiguration<MilestoneParticipation>
    {
        public void Configure(EntityTypeBuilder<MilestoneParticipation> builder)
        {
            builder.ToTable("milestone_participations");
            builder.HasKey(mp => mp.Id);
            builder.Property(mp => mp.ProjectMilestoneId).IsRequired();
            builder.Property(mp => mp.EmployeeId).IsRequired();
            builder.Property(mp => mp.IsPaid).IsRequired();

            builder.HasOne(mp => mp.ProjectMilestone)
                .WithMany(pm => pm.Participations)
                .HasForeignKey(mp => mp.ProjectMilestoneId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasIndex(mp => new { mp.ProjectMilestoneId, mp.EmployeeId }).IsUnique();

        }
    }
}