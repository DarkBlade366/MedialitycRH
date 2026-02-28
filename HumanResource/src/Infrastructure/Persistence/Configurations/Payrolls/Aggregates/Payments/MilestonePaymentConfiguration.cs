using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Domain.Features.Payrolls.Aggregates;
using Domain.Features.Payrolls.Aggregates.Payments;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Persistence.Configurations.Payrolls.Aggregates.Payments
{
    public class MilestonePaymentConfiguration : IEntityTypeConfiguration<MilestonePayment>
    {
        public void Configure(EntityTypeBuilder<MilestonePayment> builder)
        {
        builder.ToTable("milestone_payments");
        builder.HasKey(p => p.Id);
        builder.Property(p => p.PayrollId).IsRequired();
        builder.Property(p => p.MilestoneRuleId).IsRequired();
        builder.Property(p => p.Amount).IsRequired().HasPrecision(18, 2);
        builder.Property(p => p.PaidAt).IsRequired();
        }
    }
}
