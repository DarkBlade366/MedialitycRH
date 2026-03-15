using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Domain.Features.Projects.Aggregates;
using Domain.Features.Projects.Enums;
using Domain.Features.Projects.Interfaces;
using Application.Features.Redmine.Interfaces;
using Application.Common.Interfaces;

namespace Application.Features.Redmine.Handlers
{
    public class SyncRedmineMilestonesHandler
    {
        private readonly IRedmineService _redmineService;
        private readonly IProjectMilestoneRepository _repository;
        private readonly IUnitOfWork _unitOfWork;

        public SyncRedmineMilestonesHandler(
            IRedmineService redmineService,
            IProjectMilestoneRepository repository,
            IUnitOfWork unitOfWork)
        {
            _redmineService = redmineService;
            _repository = repository;
            _unitOfWork = unitOfWork;
        }

        public async Task<int> Handle(CancellationToken ct)
        {
            var allProjects = await _redmineService.GetAllProjectsAsync();
            int created = 0;

            foreach (var project in allProjects)
            {
                var redmineMilestones = await _redmineService.GetProjectMilestonesAsync(project.Id);
                if (redmineMilestones == null || redmineMilestones.Count == 0)
                    continue;

                var existing = await _repository.GetByProjectIdAsync(project.Id);
                var existingDict = existing.ToDictionary(m => m.Name, StringComparer.OrdinalIgnoreCase);

                var toAdd = new List<ProjectMilestone>();

                foreach (var m in redmineMilestones)
                {
                    var status = MapStatus(m.Status);

                    if (existingDict.TryGetValue(m.Name, out var local))
                    {
                        bool needsUpdate = false;
                        if (status == MilestoneStatus.Completed && !local.IsCompleted())
                        {
                            local.MarkAsCompleted(m.CompletedAt?.ToUniversalTime() ?? DateTime.Now);
                            needsUpdate = true;
                        }
                        else if (status == MilestoneStatus.Cancelled && !local.IsCancelled())
                        {
                            local.MarkAsCancelled();
                            needsUpdate = true;
                        }
                        else if (status == MilestoneStatus.Pending && (local.IsCompleted() || local.IsCancelled()))
                        {
                            local.Reopen();
                            needsUpdate = true;
                        }
                        
                        if (needsUpdate)
                        {
                            _repository.Update(local);
                        }
                    }
                    else
                    {
                        var milestone = new ProjectMilestone(project.Id, m.Name);
                        switch (status)
                        {
                            case MilestoneStatus.Completed:
                                milestone.MarkAsCompleted(m.CompletedAt?.ToUniversalTime() ?? DateTime.Now);
                                break;
                            case MilestoneStatus.Cancelled:
                                milestone.MarkAsCancelled();
                                break;
                            case MilestoneStatus.Pending:
                            default:
                                break;
                        }

                        toAdd.Add(milestone);
                        created++;
                    }
                }

                if (toAdd.Count > 0)
                    await _repository.AddRangeAsync(toAdd);
            }

            await _unitOfWork.SaveChangesAsync(ct);
            return created;
        }

        //Método privado que mapea el string de Redmine a nuestro enum
        private MilestoneStatus MapStatus(string redmineStatus)
        {
            if (string.IsNullOrWhiteSpace(redmineStatus))
                return MilestoneStatus.Pending;

            return redmineStatus.ToLower() switch
            {
                "open" => MilestoneStatus.Pending,
                "closed" => MilestoneStatus.Completed,
                "locked" => MilestoneStatus.Cancelled,
                _ => MilestoneStatus.Pending
            };
        }
    }
}