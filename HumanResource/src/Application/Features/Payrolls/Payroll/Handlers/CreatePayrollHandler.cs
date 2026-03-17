using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Application.Common.Interfaces;
using Application.Features.Payrolls.Payroll.Commands;
using Application.Features.Payrolls.Payroll.DTOs;
using Application.Services;
using Domain.Features.Employees.Interfaces;
using Domain.Features.Payrolls.Enums;
using Domain.Features.Payrolls.Interfaces;
using Domain.Features.Payrolls.Services.Context;
using Domain.Features.Payrolls.Services.Engines;
using Domain.Features.Projects.Enums;
using Domain.Features.Projects.Interfaces;
using Domain.Features.TimeEntries.Interfaces;

namespace Application.Features.Payrolls.Payroll.Handlers
{
    public class CreatePayrollHandler
    {
        private readonly IEmployeeRepository _employeeRepository;
        private readonly IPayrollRepository _payrollRepository;
        private readonly ITimeEntryRepository _timeEntryRepository;
        private readonly PayrollEngine _payrollEngine;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IBaseSalaryRuleRepository _baseSalaryRuleRepository;
        private readonly IOvertimeRuleRepository _overtimeRuleRepository;
        private readonly IDeductionRuleRepository _deductionRuleRepository;
        private readonly IProductivityRuleRepository _productivityRuleRepository;
        private readonly IVacationRuleRepository _vacationRuleRepository;
        private readonly IAguinaldoRuleRepository _aguinaldoRuleRepository;
        private readonly IMilestoneRuleRepository _milestoneRuleRepository;
        private readonly IProjectMilestoneRepository _projectMilestoneRepository;
        private readonly IMilestoneParticipationRepository _milestoneParticipationRepository;
        private readonly ProductivityService _productivityService;
        private readonly IProjectRuleRepository _projectRuleRepository;
        private readonly IProjectRepository _projectRepository;
        private readonly ICacheService _cache;

        public CreatePayrollHandler(
            IEmployeeRepository employeeRepository,
            IPayrollRepository payrollRepository,
            ITimeEntryRepository timeEntryRepository,
            PayrollEngine payrollEngine,
            IUnitOfWork unitOfWork,
            IBaseSalaryRuleRepository baseSalaryRuleRepository,
            IOvertimeRuleRepository overtimeRuleRepository,
            IDeductionRuleRepository deductionRuleRepository,
            IProductivityRuleRepository productivityRuleRepository,
            IVacationRuleRepository vacationRuleRepository,
            IAguinaldoRuleRepository aguinaldoRuleRepository,
            IMilestoneRuleRepository milestoneRuleRepository,
            IProjectMilestoneRepository projectMilestoneRepository,
            IMilestoneParticipationRepository milestoneParticipationRepository,
            ProductivityService productivityService,
            IProjectRuleRepository projectRuleRepository,
            IProjectRepository projectRepository,
            ICacheService cache)
        {
            _employeeRepository = employeeRepository;
            _payrollRepository = payrollRepository;
            _timeEntryRepository = timeEntryRepository;
            _payrollEngine = payrollEngine;
            _unitOfWork = unitOfWork;
            _baseSalaryRuleRepository = baseSalaryRuleRepository;
            _overtimeRuleRepository = overtimeRuleRepository;
            _deductionRuleRepository = deductionRuleRepository;
            _productivityRuleRepository = productivityRuleRepository;
            _vacationRuleRepository = vacationRuleRepository;
            _aguinaldoRuleRepository = aguinaldoRuleRepository;
            _milestoneRuleRepository = milestoneRuleRepository;
            _projectMilestoneRepository = projectMilestoneRepository;
            _milestoneParticipationRepository = milestoneParticipationRepository;
            _productivityService = productivityService;
            _projectRepository = projectRepository;
            _projectRuleRepository = projectRuleRepository;
            _cache = cache;
        }

        public async Task<PayrollResponse> Handle(
            CreatePayrollCommand command,
            CancellationToken cancellationToken)
        {
            Console.WriteLine($"[HANDLER] Creando nómina para período: {command.periodStart} - {command.periodEnd}");

            //Periodo invalido
            if (command.periodStart >= command.periodEnd)
                throw new Exception("Invalid payroll period.");

            //Solapamiento entre pagos
            var overlap = await _payrollRepository
                .ExistsOverlappingPayroll(
                    command.employeeId, 
                    command.periodStart, 
                    command.periodEnd);

            if (overlap)
                throw new Exception("Payroll period overlaps with existing payroll.");
            
            var pendingEntries = await _timeEntryRepository
                .HasPendingEntries(
                    command.employeeId,
                    command.periodStart,
                    command.periodEnd);
            
            if (pendingEntries)
                throw new Exception("There are time entries pending approval for this period.");

            //empleado inexistente
            var employee = await _employeeRepository.GetByIdAsync(command.employeeId);

            if (employee == null)
                throw new Exception("Employee not found.");

            //buscando cuantas horas hizo
            var workedHours = await _timeEntryRepository
                .GetWorkedHours(
                    command.employeeId,
                    command.periodStart,
                    command.periodEnd);

            // Métrica de productividad ponderada por tipo de actividad
            var productivityMetric = await _productivityService.CalculateProductivityMetric(
                command.employeeId,
                command.periodStart,
                command.periodEnd);

            //Vacaciones usadas
            var vacationDaysUsed = employee.VacationBalance.UsedDays;
            
            // Base Salary
            var baseSalaryRules = (await _baseSalaryRuleRepository.GetAllAsync())
                .Where(r => r.Role == employee.Role && r.IsActive)
                .ToList();

            // Project
            var projectRules = (await _projectRuleRepository.GetAllAsync())
                .Where(r => r.IsActive)
                .ToList();

            var completedProjects = (await _projectRepository.GetAllAsync())
                .Where(p =>
                    p.Status == ProjectStatus.Completed &&
                    p.CompletedAt.HasValue &&
                    p.CompletedAt.Value >= command.periodStart &&
                    p.CompletedAt.Value <= command.periodEnd)
                .ToList();

            //Time Entry
            var timeEntries = await _timeEntryRepository
                .GetByPeriodAsync(
                    command.periodStart,
                    command.periodEnd);
            
            // Overtime
            var overtimeRules = (await _overtimeRuleRepository.GetAllAsync())
                .Where(r => r.IsActive)
                .ToList();
            
            // Deductions
            var deductionRules = (await _deductionRuleRepository.GetAllAsync())
                .Where(r => r.IsActive)
                .ToList();
            
            // Productivity
            var productivityRule = (await _productivityRuleRepository.GetAllAsync())
                .FirstOrDefault(r => r.IsActive);
            
            // Vacation
            var vacationRule = (await _vacationRuleRepository.GetAllAsync())
                .FirstOrDefault(r => r.IsActive);
            
            // Aguinaldo
            var aguinaldoRule = (await _aguinaldoRuleRepository.GetAllAsync())
                .FirstOrDefault(r => r.IsActive);
            
            // Milestone Rules
            var milestoneRules = (await _milestoneRuleRepository.GetAllAsync())
                .Where(r => r.IsActive)
                .ToList();

            //Milestone Pariticipation
            var participations = await _milestoneParticipationRepository
                .GetByEmployeeIdAsync(command.employeeId);
            
            // Project Milestones
            var projectMilestones = await _projectMilestoneRepository
                .GetAllAsync();

            var employeeParticipations = participations
                .Where(p =>
                    !p.IsPaid &&
                    p.ProjectMilestone != null &&
                    p.ProjectMilestone.CompletedAt.HasValue &&
                    p.ProjectMilestone.CompletedAt.Value >= command.periodStart &&
                    p.ProjectMilestone.CompletedAt.Value <= command.periodEnd)
                .ToList();

            var context = new PayrollCalculationContext(
                baseSalaryRules,
                employee.Role,

                workedHours,

                overtimeRules,
                deductionRules,

                productivityMetric,
                productivityRule,

                vacationRule,
                employee.VacationBalance,
                vacationDaysUsed,

                aguinaldoRule,
                employee.AguinaldoBalance,

                milestoneRules,
                employeeParticipations,
                projectMilestones,

                projectRules,
                completedProjects,
                timeEntries,

                command.periodStart,
                command.periodEnd
            );

            Console.WriteLine($"[HANDLER] Contexto preparado, iniciando cálculo...");

            var payroll = _payrollEngine.Calculate(command.employeeId, context);

            await _payrollRepository.AddAsync(payroll);

            await _unitOfWork.SaveChangesAsync(cancellationToken);

            await _cache.RemoveAsync("payrolls:all");
            await _cache.RemoveAsync($"payroll:{payroll.EmployeeId}");

            Console.WriteLine($"[HANDLER] Nómina creada exitosamente");
            return new PayrollResponse
            {
                Id = payroll.Id,
                EmployeeId = payroll.EmployeeId,
                PeriodStart = payroll.PeriodStart,
                PeriodEnd = payroll.PeriodEnd,
                GrossAmount = payroll.GrossAmount,
                TotalDeductions = payroll.TotalDeductions,
                NetAmount = payroll.NetAmount,
                Status = payroll.Status.ToString(),
                Components = payroll.Components
                    .Select(c => new PayrollComponentResponse
                    {
                        Type = c.Type.ToString(),
                        Category = c.Category.ToString(),
                        Description = c.Description,
                        Amount = c.Amount
                    })
                    .ToList()
            };
        }
    }
}