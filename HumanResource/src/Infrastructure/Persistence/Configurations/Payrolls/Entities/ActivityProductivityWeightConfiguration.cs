using Domain.Features.Payrolls.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Persistence.Configurations.Payrolls.Entities
{
    public class ActivityProductivityWeightConfiguration : IEntityTypeConfiguration<ActivityProductivityWeight>
    {
        public void Configure(EntityTypeBuilder<ActivityProductivityWeight> builder)
        {
            builder.ToTable("activity_productivity_weights");
            builder.HasKey(x => x.Id);
            builder.Property(x => x.RedmineActivityId).IsRequired();
            builder.Property(x => x.ActivityName).IsRequired().HasMaxLength(100);
            builder.Property(x => x.Weight).IsRequired().HasPrecision(3, 2);
            builder.Property(x => x.IsActive).IsRequired();
            builder.HasIndex(x => x.RedmineActivityId).IsUnique();
        }
    }
}
