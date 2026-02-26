using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Domain.Enums;
using Domain.Models;

namespace Domain.Interfaces
{
    public interface ISalaryConfigurationRepository
    {
        Task<SalaryConfiguration?> GetByRoleAsync(EmployeeRole role);
    }
}