using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Application.Redmine.Interfaces;
using Domain.Interfaces;
using Domain.Models;

namespace Application.Redmine.Handlers
{
    public class SyncRedmineProjectsHandler
    {
        private readonly IRedmineService _redmineService;
        private readonly IProjectRepository _projectRepository;

        public SyncRedmineProjectsHandler(IRedmineService redmineService, IProjectRepository projectRepository)
        {
            _redmineService = redmineService;
            _projectRepository = projectRepository;
        }

        public async Task<int> Handle()
        {
            var redmineProjects = await _redmineService.GetProjectsAsync();
            int created = 0;

            foreach (var rp in redmineProjects)
            {
                var existing = await _projectRepository
                    .GetByRedmineIdAsync(rp.Id);

                if (existing == null)
                {
                    var project = new Project(rp.Id, rp.Name);
                    await _projectRepository.AddAsync(project);
                    created++;
                }
                else
                {
                    existing.UpdateName(rp.Name);
                }
            }

            await _projectRepository.SaveChangesAsync();
            return created;
        }
    }
}