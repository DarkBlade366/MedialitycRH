using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Application.Features.Redmine.Interfaces;
using Domain.Features.Projects.Interfaces;
using Domain.Features.Projects.Aggregates;
using Application.Common.Interfaces;

namespace Application.Features.Redmine.Handlers
{
    public class SyncRedmineProjectsHandler
    {
        private readonly IRedmineService _redmineService;
        private readonly IProjectRepository _projectRepository;
        private readonly IUnitOfWork _unitOfWork;

        public SyncRedmineProjectsHandler(
            IRedmineService redmineService, 
            IProjectRepository projectRepository,
            IUnitOfWork unitOfWork)
        {
            _redmineService = redmineService;
            _projectRepository = projectRepository;
            _unitOfWork = unitOfWork;
        }

        public async Task<int> Handle(CancellationToken ct)
        {
            var redmineProjects = await _redmineService.GetProjectsAsync();
            int created = 0;

            var localProjects = await _projectRepository.GetAllAsync();

            foreach (var rp in redmineProjects)
            {
                var existing = localProjects.FirstOrDefault(p => p.RedmineProjectId == rp.Id);

                if (existing == null)
                {
                    var project = new Project(rp.Id, rp.Name);
                    await _projectRepository.AddAsync(project);
                    created++;
                }
                else
                {
                    if (existing.Name != rp.Name)
                    {
                        existing.UpdateName(rp.Name);
                        _projectRepository.Update(existing);
                    }
                }
            }

            var redmineIds = redmineProjects.Select(r => r.Id).ToHashSet();
            var toDelete = localProjects.Where(p => !redmineIds.Contains(p.RedmineProjectId)).ToList();

            foreach (var del in toDelete)
            {
                _projectRepository.Delete(del);
            }

            await _unitOfWork.SaveChangesAsync(ct);

            return created;
        }
    }
}
