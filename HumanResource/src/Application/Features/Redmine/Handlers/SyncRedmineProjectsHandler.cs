using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Application.Features.Redmine.Interfaces;
using Domain.Features.Projects.Interfaces;
using Domain.Features.Projects.Aggregates;
using Application.Common.Interfaces;
using Domain.Features.Projects.Enums;

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
            var localProjects = await _projectRepository.GetAllAsync();

            int created = 0;

            foreach (var rp in redmineProjects)
            {
                Console.WriteLine($"Redmine Project received: Id={rp.Id}, Name={rp.Name}, Status={rp.Status}");

                var existing = localProjects.FirstOrDefault(p => p.RedmineProjectId == rp.Id);
                var status = MapStatus(rp.Status);

                Console.WriteLine($"Mapped status for project {rp.Name} = {status}");

                if (existing == null)
                {
                    var project = new Project(rp.Id, rp.Name, status);
                    await _projectRepository.AddAsync(project);
                    created++;
                }
                else
                {
                    Console.WriteLine($"Found existing project: Id={existing.RedmineProjectId}, Name={existing.Name}, Status={existing.Status}");
                    
                    bool needsUpdate = false;
                    if (existing.Name != rp.Name)
                    {
                        Console.WriteLine($"Updating project name from '{existing.Name}' to '{rp.Name}'");
                        existing.UpdateName(rp.Name);
                        needsUpdate = true;
                    }
                    if (existing.Status != status)
                    {
                        Console.WriteLine($"Updating project status from '{existing.Status}' to '{status}'");
                        existing.UpdateStatus(status);
                        needsUpdate = true;
                    }
                    
                    if (needsUpdate)
                    {
                        Console.WriteLine($"Calling Update for project {existing.RedmineProjectId}");
                        _projectRepository.Update(existing);
                        Console.WriteLine($"Update called for project {existing.RedmineProjectId}");
                    }
                    else
                    {
                        Console.WriteLine($"No updates needed for project {existing.RedmineProjectId}");
                    }
                }
            }

            var redmineIds = redmineProjects.Select(r => r.Id).ToHashSet();
            foreach (var del in localProjects.Where(p => !redmineIds.Contains(p.RedmineProjectId)))
            {
                Console.WriteLine($"Project not found in Redmine: Id={del.RedmineProjectId}, Name={del.Name}, CurrentStatus={del.Status}");
                if (del.Status != ProjectStatus.Completed)
                {
                    del.UpdateStatus(ProjectStatus.Cancelled);
                    _projectRepository.Update(del);
                    Console.WriteLine($"Project {del.Name} marked as Cancelled");
                }
            }

            await _unitOfWork.SaveChangesAsync(ct);

            return created;
        }

        private ProjectStatus MapStatus(int status)
        {
            return status switch
            {
                1 => ProjectStatus.Active,      // Active projects
                5 => ProjectStatus.Completed,   // Closed projects
                9 => ProjectStatus.Cancelled,  // Archived projects
                _ => ProjectStatus.Active      // Default to Active for unknown statuses
            };
        }
    }
}
