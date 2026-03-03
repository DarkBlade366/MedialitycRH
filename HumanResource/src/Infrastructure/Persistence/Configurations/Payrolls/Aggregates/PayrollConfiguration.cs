using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Domain.Features.Payrolls.Aggregates;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Persistence.Configurations.Payrolls.Aggregates
{
    public class PayrollConfiguration : IEntityTypeConfiguration<Payroll>
    {
        public void Configure(EntityTypeBuilder<Payroll> builder)
        {
            builder.ToTable("payrolls");
            builder.HasKey(p => p.Id);
            builder.Property(p => p.EmployeeId).IsRequired();
            builder.Property(p => p.PeriodStart).IsRequired();
            builder.Property(p => p.PeriodEnd).IsRequired();
            builder.Property(p => p.Status).IsRequired().HasConversion<string>();

            builder.HasMany(p => p.Components)
                    .WithOne()
                    .HasForeignKey("PayrollId")
                    .OnDelete(DeleteBehavior.Cascade);

            builder.Property(p => p.GrossAmount).IsRequired();
            builder.Property(p => p.TotalDeductions).IsRequired();
            builder.Property(p => p.NetAmount).IsRequired();
        }
    }
}
