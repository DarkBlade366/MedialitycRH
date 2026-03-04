using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Domain.Features.Employees.Interfaces;
using Domain.Features.Payrolls.Rules;
using Domain.Features.Payrolls.Interfaces;
using Application.Common.Interfaces;

namespace Application.Services
{
    public class VacationAccrualService
    {
        private readonly IEmployeeRepository _employeeRepo;
        private readonly IVacationRuleRepository _ruleRepo;
        private readonly IUnitOfWork _unitOfWork;

        public VacationAccrualService(
            IEmployeeRepository employeeRepo,
            IVacationRuleRepository ruleRepo,
            IUnitOfWork unitOfWork)
        {
            _employeeRepo = employeeRepo;
            _ruleRepo = ruleRepo;
            _unitOfWork = unitOfWork;
        }

        public async Task AccrueVacationsAsync()
        {
            var employees = await _employeeRepo.GetAllActiveAsync();
            var rules = await _ruleRepo.GetAllAsync();

            foreach (var emp in employees)
            {
                var rule = rules.FirstOrDefault(r => r.IsActive);
                if (rule == null)
                    continue;

                if (emp.VacationBalance.HasAccruedThisMonth())
                    continue;

                emp.AccrueVacationDays(rule.AccrualRatePerMonth);
            }

            await _unitOfWork.SaveChangesAsync();
        }
    }
}
