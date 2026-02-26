using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Domain.Interfaces;
using Application.SalaryConfigurations.Commands;

namespace Application.SalaryConfigurations.Handlers
{   
    public class UpdateSalaryConfigurationHandler
    {
        private readonly ISalaryConfigurationRepository _repository;

        public UpdateSalaryConfigurationHandler(ISalaryConfigurationRepository repository)
        {
            _repository = repository;
        }

        public async Task HandleAsync(UpdateSalaryConfigurationCommand command, CancellationToken ct)
        {
            var config = await _repository.GetByRoleAsync(command.Role)
                ?? throw new Exception("Salary configuration not found.");

            config.UpdateBaseRate(command.NewHourlyRate);

            await _repository.UpdateAsync(config);
        }
    }
}