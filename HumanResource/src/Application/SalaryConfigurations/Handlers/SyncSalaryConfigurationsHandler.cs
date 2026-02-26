using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Domain.Enums;
using Domain.Interfaces;
using Domain.Models;

namespace Application.SalaryConfigurations.Handlers
{
    public class SyncSalaryConfigurationsHandler
    {
        private readonly ISalaryConfigurationRepository _repository;

        public SyncSalaryConfigurationsHandler(ISalaryConfigurationRepository repository)
        {
            _repository = repository;
        }

        public async Task<int> HandleAsync(CancellationToken ct)
        {
            var existingConfigs = await _repository.GetAllAsync();
            var roles = Enum.GetValues(typeof(EmployeeRole)).Cast<EmployeeRole>();

            int createdCount = 0;

            foreach (var role in roles)
            {
                var exists = existingConfigs
                    .Any(x => x.Role == role);

                if (!exists)
                {
                    var config = new SalaryConfiguration(role, 0);
                    await _repository.AddAsync(config);
                    createdCount++;
                }
            }
            return createdCount;
        }
    }
}