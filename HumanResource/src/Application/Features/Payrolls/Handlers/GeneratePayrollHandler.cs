// using System;
// using System.Linq;
// using System.Threading.Tasks;
// using Application.Features.Payrolls.Commands;
// using Domain.Features.Employees.Interfaces;
// using Domain.Features.Payrolls.Interfaces;
// using Domain.Features.Payrolls.Services.Context;
// using Domain.Features.Payrolls.Services.Engines;
// using Domain.Features.TimeEntries.Interfaces;


// namespace Application.Features.Payrolls.Handlers
// {
//     public class GeneratePayrollHandler
// { 
//         private readonly IEmployeeRepository _employeeRepository;
//         private readonly ITimeEntryRepository _timeEntryRepository;
//         private readonly IPayrollRepository _payrollRepository;
//         private readonly PayrollEngine _payrollEngine;

//         public GeneratePayrollHandler(
//             IEmployeeRepository employeeRepository,
//             ITimeEntryRepository timeEntryRepository,
//             IPayrollRepository payrollRepository,
//             PayrollEngine payrollEngine)
//         {
//             _employeeRepository = employeeRepository;
//             _timeEntryRepository = timeEntryRepository;
//             _payrollRepository = payrollRepository;
//             _payrollEngine = payrollEngine;
//         }

//         public async Task<Guid> Handle(GeneratePayrollCommand command)
//         {
//             if (await _payrollRepository.ExistsAsync(command.EmployeeId, command.From, command.To))
//                 throw new Exception("Payroll already exists for this period.");

//             var employee = await _employeeRepository.GetByIdAsync(command.EmployeeId)
//                 ?? throw new Exception("Employee not found.");

//             var timeEntries = await _timeEntryRepository.GetByEmployeeAndPeriodAsync(
//                 command.EmployeeId,
//                 command.From,
//                 command.To);

//             if (!timeEntries.Any())
//                 throw new Exception("No time entries found.");

//             // 4️⃣ Construir PayrollCalculationContext
//             var context = new PayrollCalculationContext(
//                 baseSalaryRule: employee.BaseSalaryRule,
//                 hourlyRate: employee.HourlyRate,
//                 totalWorkedHours: timeEntries.Sum(te => te.Hours),
//                 overtimeRules: employee.OvertimeRules,
//                 deductionRules: employee.DeductionRules,
//                 productivityMetric: employee.ProductivityMetric,
//                 productivityRule: employee.ProductivityRule,
//                 vacationRule: employee.VacationRule,
//                 vacationBalance: employee.VacationBalance,
//                 vacationDaysUsed: employee.VacationDaysUsed,
//                 aguinaldoRule: employee.AguinaldoRule,
//                 aguinaldoBalance: employee.AguinaldoBalance,
//                 milestoneRules: employee.MilestoneRules,
//                 completedMilestones: employee.CompletedMilestones,
//                 periodStart: command.From,
//                 periodEnd: command.To
//             );

//             var payroll = _payrollEngine.Calculate(employee.Id, context);

//             await _payrollRepository.AddAsync(payroll);

//             return payroll.Id;
//         }
//     }
// }
