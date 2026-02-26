using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Domain.Models
{
    public class Project
    {
        public Guid Id { get; private set; }
        public int RedmineProjectId { get; private set; }
        public string? Name { get; private set; }

        private Project() { }

        public Project(int redmineProjectId, string name)
        {
            Id = Guid.NewGuid();
            RedmineProjectId = redmineProjectId;
            Name = name;
        }

        public void UpdateName(string name)
        {
            Name = name;
        }
    }
}