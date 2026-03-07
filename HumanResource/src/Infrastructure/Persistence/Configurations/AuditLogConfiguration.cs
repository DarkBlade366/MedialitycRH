using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Domain.Common;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Persistence.Configurations
{
    public class AuditLogConfiguration: IEntityTypeConfiguration<AuditLog>
    {
        public void Configure(EntityTypeBuilder<AuditLog> builder)
        {
            builder.ToTable("audit_logs");
            builder.HasKey(a => a.Id);
            builder.Property(a => a.EntityName).IsRequired();
            builder.Property(a => a.EntityId).IsRequired();
            builder.Property(a => a.Action).IsRequired();
            builder.Property(a => a.UserName).IsRequired();
            builder.Property(a => a.Timestamp).IsRequired();
            builder.Property(a => a.OldValues).HasColumnType("jsonb");
            builder.Property(a => a.NewValues).HasColumnType("jsonb");
        }
    }
}