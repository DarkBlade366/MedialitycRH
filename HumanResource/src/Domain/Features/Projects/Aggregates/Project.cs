using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Domain.Features.Projects.Aggregates
{
    public class Project
    {
        public Guid Id { get; private set; }
        public int RedmineProjectId { get; private set; }
        public string? Name { get; private set; }

        private Project() { }

        public Project(int redmineProjectId, string name)
        {
            if (redmineProjectId <= 0)
                throw new ArgumentException("RedmineProjectId must be greater than zero.");

            if (string.IsNullOrWhiteSpace(name))
                throw new ArgumentException("Project name is required.");

            Id = Guid.NewGuid();
            RedmineProjectId = redmineProjectId;
            Name = name;
        }

        public void UpdateName(string name)
        {
            if (string.IsNullOrWhiteSpace(name))
                throw new ArgumentException("Project name is required.");

            Name = name;
        }
    }
}
