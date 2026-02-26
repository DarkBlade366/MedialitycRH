using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Domain.Enums;
using Domain.Interfaces;
using Domain.Models;

namespace Domain.Services
{
    public class PayrollEngine : IPayrollEngine
    {
        private readonly ISalaryConfigurationRepository _salaryRepository;
        private readonly IProjectBonusConfigurationRepository _bonusRepository;

        public PayrollEngine(ISalaryConfigurationRepository salaryRepository, IProjectBonusConfigurationRepository bonusRepository)
        {
            _salaryRepository = salaryRepository;
            _bonusRepository = bonusRepository;
        }

        public async Task<Payroll> GenerateAsync(Employee employee, DateTime from, DateTime to, List<TimeEntry> entries)
        {
            if (from > to)
                throw new ArgumentException("Invalid payroll period.");

            if (entries == null || !entries.Any())
                throw new InvalidOperationException("No time entries found for this period.");

            var salaryConfig = await _salaryRepository
                .GetByRoleAsync(employee.Role)
                ?? throw new InvalidOperationException("Salary configuration not found for employee role.");

            var projectBonuses = await _bonusRepository.GetAllAsync();

            var payroll = new Payroll(employee.Id, from, to);

            var grouped = entries
                .GroupBy(x => new { x.RedmineProjectId, x.ProjectName });

            decimal totalBonusAmount = 0;

            foreach (var group in grouped)
            {
                decimal hours = group.Sum(x => (decimal)x.Hours);

                var bonusConfig = projectBonuses
                    .FirstOrDefault(b =>
                        b.RedmineProjectId == group.Key.RedmineProjectId);

                decimal finalRate = salaryConfig.BaseHourlyRate;

                if (bonusConfig != null)
                {
                    finalRate += bonusConfig.ExtraHourlyRate;
                    totalBonusAmount += bonusConfig.ExtraHourlyRate * hours;
                }

                var line = new PayrollLine(
                    group.Key.RedmineProjectId,
                    group.Key.ProjectName,
                    hours,
                    finalRate);

                payroll.AddLine(line);
            }

            decimal baseAmount =
                payroll.TotalHours * salaryConfig.BaseHourlyRate;

            payroll.AddComponent(new PayrollComponent(
                PayrollComponentType.BaseSalary,
                "Base salary calculated from total hours",
                baseAmount));

            if (totalBonusAmount > 0)
            {
                payroll.AddComponent(new PayrollComponent(
                    PayrollComponentType.ProjectBonus,
                    "Project bonus based on extra hourly rates",
                    totalBonusAmount));
            }

            if (payroll.TotalHours > 160)
            {
                decimal overtimeHours = payroll.TotalHours - 160;

                decimal overtimeAmount =
                    overtimeHours *
                    salaryConfig.BaseHourlyRate *
                    0.5m;

                payroll.AddComponent(new PayrollComponent(
                    PayrollComponentType.Overtime,
                    $"Overtime {overtimeHours}h",
                    overtimeAmount));
            }

            return payroll;
        }
    }
}