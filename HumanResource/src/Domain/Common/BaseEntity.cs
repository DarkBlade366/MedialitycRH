using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Domain.Common
{
    public abstract class BaseEntity
    {
        public DateTime CreatedAt { get; protected set; }
        public DateTime? UpdatedAt { get; protected set; }
        public string? CreatedBy { get; protected set; }
        public string? UpdatedBy { get; protected set; }

        protected BaseEntity()
        {
            CreatedAt = DateTime.UtcNow;
        }
        public void MarkCreated(string userName)
        {
            CreatedAt = DateTime.UtcNow;
            CreatedBy = userName;
            MarkUpdated(userName);
        }
        public void MarkUpdated(string userName)
        {
            UpdatedAt = DateTime.UtcNow;
            UpdatedBy = userName;
        }
    }
}