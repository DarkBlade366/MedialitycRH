using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Domain.Models;

namespace Domain.Services
{
    public interface IPayrollEngine
    {
        Task<Payroll> GenerateAsync(Employee employee, DateTime from, DateTime to, List<TimeEntry> timeEntries);
    }
}