using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Domain.Common
{
    public class AuditLog
    {
        public Guid Id { get; set; }
        public string EntityName { get; set; } = null!;
        public string EntityId { get; set; } = null!;
        public string Action { get; set; } = null!; // Create, Update, Delete
        public string? OldValues { get; set; } // JSON
        public string? NewValues { get; set; } // JSON
        public string UserName { get; set; } = null!;
        public DateTime Timestamp { get; set; } = DateTime.UtcNow;
    }
}