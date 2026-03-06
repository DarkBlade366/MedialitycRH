using System.Collections.Generic;
using System.Threading.Tasks;
using Application.Features.Redmine.DTOs;
using Application.Features.Redmine.Interfaces;

namespace Application.Features.Redmine.Handlers
{
    public class GetRedmineTimeEntryActivitiesHandler
    {
        private readonly IRedmineService _redmineService;

        public GetRedmineTimeEntryActivitiesHandler(IRedmineService redmineService)
        {
            _redmineService = redmineService;
        }

        public async Task<List<RedmineTimeEntryActivityDto>> HandleAsync()
        {
            return await _redmineService.GetTimeEntryActivitiesAsync();
        }
    }
}
