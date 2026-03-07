using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Text.Json;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Application.Common.Interfaces;

namespace Infrastructure.Persistence
{
    public class AuditInterceptor : SaveChangesInterceptor
    {
        private readonly ICurrentUserService _currentUser;

        public AuditInterceptor(ICurrentUserService currentUser)
        {
            _currentUser = currentUser;
        }

        public override ValueTask<InterceptionResult<int>> SavingChangesAsync(
            DbContextEventData eventData,
            InterceptionResult<int> result,
            CancellationToken cancellationToken = default)
        {
            var context = eventData.Context;
            if (context == null) return base.SavingChangesAsync(eventData, result, cancellationToken);

            var auditEntries = context.ChangeTracker
                .Entries()
                .Where(e => e.Entity is Domain.Common.BaseEntity && 
                            (e.State == EntityState.Modified || e.State == EntityState.Added || e.State == EntityState.Deleted))
                .Select(e =>
                {
                    var entity = e.Entity;
                    var entry = new Domain.Common.AuditLog
                    {
                        EntityName = entity.GetType().Name,
                        EntityId = (entity as Domain.Common.BaseEntity)?.GetHashCode().ToString() ?? Guid.NewGuid().ToString(),
                        Action = e.State.ToString(),
                        UserName = _currentUser.UserName,
                        Timestamp = DateTime.UtcNow,
                        OldValues = e.State == EntityState.Modified ? JsonSerializer.Serialize(e.OriginalValues.ToObject()) : null,
                        NewValues = e.State != EntityState.Deleted ? JsonSerializer.Serialize(e.CurrentValues.ToObject()) : null
                    };
                    return entry;
                }).ToList();

            if (auditEntries.Any())
            {
                context.Set<Domain.Common.AuditLog>().AddRange(auditEntries);
            }

            return base.SavingChangesAsync(eventData, result, cancellationToken);
        }
    }
}