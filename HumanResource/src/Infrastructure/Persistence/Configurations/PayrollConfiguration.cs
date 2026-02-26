using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Domain.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Persistence.Configurations
{
    public class PayrollConfiguration : IEntityTypeConfiguration<Payroll>
    {
        public void Configure(EntityTypeBuilder<Payroll> builder)
        {
            builder.ToTable("payrolls");
            builder.HasKey(p => p.Id);
            builder.Property(p => p.TotalHours).HasColumnType("decimal(12,2)");
            builder.Property(p => p.TotalAmount).HasColumnType("decimal(14,2)");
            builder.Property(p => p.Status).HasConversion<string>();
            builder.HasIndex(p => new { p.EmployeeId, p.PeriodFrom, p.PeriodTo }).IsUnique();
            
            builder.HasMany(p => p.Lines)
                    .WithOne()
                    .HasForeignKey(l => l.PayrollId)
                    .OnDelete(DeleteBehavior.Cascade);

            builder.HasMany(p => p.Components)
                    .WithOne()
                    .HasForeignKey(c => c.PayrollId)
                    .OnDelete(DeleteBehavior.Cascade);
        }
    }
}