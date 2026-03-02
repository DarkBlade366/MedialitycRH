using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Domain.Features.Payrolls.Aggregates.Payments;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Persistence.Configurations.Payrolls.Aggregates.Payments
{
    public class DeductionPaymentConfiguration : IEntityTypeConfiguration<DeductionPayment>
    {
        public void Configure(EntityTypeBuilder<DeductionPayment> builder)
        {
            builder.ToTable("deduction_payments");
            builder.HasKey(p => p.Id);
            builder.Property(p => p.Amount).IsRequired().HasColumnType("decimal(18,2)");
            builder.Property(p => p.AppliedAt).IsRequired();
            builder.Property(p => p.PayrollId).IsRequired();
            builder.Property(p => p.DeductionRuleId).IsRequired();
        }
    }
}