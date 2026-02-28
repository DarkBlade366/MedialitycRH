using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Domain.Common;

namespace Domain.Features.Payrolls.Rules
{
    public abstract class PayrollRule : BaseEntity
    {
        public Guid Id { get; protected set; }
        public string Name { get; protected set; }
        public bool IsActive { get; protected set; }

        protected PayrollRule(string name)
        {
            if (string.IsNullOrWhiteSpace(name))
                throw new ArgumentException("Rule name cannot be empty.");

            Id = Guid.NewGuid();
            Name = name;
            IsActive = true;
        }

        public void Deactivate()
        {
            IsActive = false;
            MarkUpdated();
        }

        public void Activate()
        {
            IsActive = true;
            MarkUpdated();
        }
    }
}
