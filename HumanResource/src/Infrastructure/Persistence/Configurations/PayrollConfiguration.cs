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
            builder.Property(p => p.EmployeeId).IsRequired();
            builder.Property(p => p.From).IsRequired();
            builder.Property(p => p.To).IsRequired();
            builder.Property(p => p.TotalHours).IsRequired().HasColumnType("decimal(10,2)");
            builder.Property(p => p.TotalAmount).IsRequired().HasColumnType("decimal(10,2)");
            builder.Property(p => p.GeneratedAt).IsRequired();
            builder.Property(p => p.CreatedAt).IsRequired();
            builder.Property(p => p.UpdatedAt);
            
            builder.HasOne(p => p.Employee)
                    .WithMany()
                    .HasForeignKey(p => p.EmployeeId)
                    .OnDelete(DeleteBehavior.Restrict);

            //No permitir duplicados para mismo empleado y periodo
            builder.HasIndex(p => new { p.EmployeeId, p.From, p.To })
                .IsUnique();
        }
    }
}