using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Domain.Features.Employees.Aggregates;
using Domain.Features.Employees.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Persistence.Configurations.Employees
{
    public class EmployeeConfiguration : IEntityTypeConfiguration<Employee>
    {
        public void Configure(EntityTypeBuilder<Employee> builder)
        {
            builder.ToTable("employees");
            builder.HasKey(e => e.Id);
            builder.Property(e => e.FullName).IsRequired().HasMaxLength(100);
            builder.Property(e => e.Email).IsRequired().HasMaxLength(100);
            builder.HasIndex(e => e.Email).IsUnique();
            builder.Property(e => e.Role).IsRequired().HasConversion<string>();
            builder.Property(e => e.IsActive).IsRequired();
            builder.Property(e => e.PasswordHash).IsRequired().HasMaxLength(200);
            builder.Property(e => e.CreatedAt).IsRequired();
            builder.Property(e => e.UpdatedAt);

            builder.HasOne(e => e.AguinaldoBalance)
                .WithOne()
                .HasForeignKey<EmployeeAguinaldoBalance>(b => b.EmployeeId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasOne(e => e.VacationBalance)
                .WithOne()
                .HasForeignKey<EmployeeVacationBalance>(b => b.EmployeeId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}