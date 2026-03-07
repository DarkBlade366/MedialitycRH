using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Domain.Features.Payrolls.Aggregates.Payments;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Persistence.Configurations.Payrolls.Aggregates.Payments
{
    public class ProjectPaymentConfiguration : IEntityTypeConfiguration<ProjectPayment>
    {
        public void Configure(EntityTypeBuilder<ProjectPayment> builder)
        {
            builder.ToTable("project_payments");
            builder.HasKey(p => p.Id);
            builder.Property(p => p.Amount).IsRequired();
            builder.Property(p => p.PaidAt).IsRequired();
            builder.Property(p => p.PayrollId).IsRequired();
            builder.Property(p => p.RedmineProjectId).IsRequired();
        }
    }
}