using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Domain.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Persistence.Configurations
{
    public class RoleSalaryConfiguration : IEntityTypeConfiguration<RoleSalary>
    {
        public void Configure(EntityTypeBuilder<RoleSalary> builder)
        {
            builder.ToTable("role_salaries");
            builder.HasKey(r => r.Id);
            builder.Property(r => r.Role).IsRequired().HasConversion<string>();
            builder.Property(r => r.BaseHourlyRate).IsRequired().HasColumnType("decimal(10,2)");
            builder.Property(r => r.CreatedAt).IsRequired();
            builder.Property(r => r.UpdatedAt);
            builder.HasIndex(r => r.Role).IsUnique();
        }
    }
}