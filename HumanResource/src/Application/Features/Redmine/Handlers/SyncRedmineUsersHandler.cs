using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Application.Features.Redmine.Interfaces;
using Domain.Common.Security;
using Domain.Features.Employees.Enums;
using Domain.Features.Employees.Interfaces;
using Domain.Features.Employees.Aggregates;
using Application.Common.Interfaces;

namespace Application.Features.Redmine.Handlers
{
    public class SyncRedmineUsersHandler
{
    private readonly IRedmineService _redmineService;
    private readonly IEmployeeRepository _employeeRepository;
    private readonly IUnitOfWork _unitOfWork;

    public SyncRedmineUsersHandler(IRedmineService redmineService, 
        IEmployeeRepository employeeRepository,
        IUnitOfWork unitOfWork)
    {
        _redmineService = redmineService;
        _employeeRepository = employeeRepository;
        _unitOfWork = unitOfWork;
    }
        public async Task<int> Handle(CancellationToken ct)
        {
            var redmineUsers = await _redmineService.GetUsersAsync();

            if (!redmineUsers.Any())
                return 0;

            var emails = redmineUsers
                .Where(x => !string.IsNullOrWhiteSpace(x.Email))
                .Select(x => x.Email!)
                .ToList();

            var existingEmails = await _employeeRepository.GetExistingEmailsAsync(emails);
            var existingEmployees = await _employeeRepository.GetPagedAsync(1, int.MaxValue);
            var existingEmployeesList = existingEmployees.Item1.ToList();

            var existingEmailsSet = existingEmails.ToHashSet();
            var newEmployees = new List<Employee>();

            foreach (var user in redmineUsers)
            {
                if (string.IsNullOrWhiteSpace(user.Email))
                    continue;

                var employee = existingEmployeesList.FirstOrDefault(e => e.Email == user.Email);

                if (employee == null)
                {
                    var newEmployee = new Employee(
                        $"{user.FirstName} {user.LastName}",
                        user.Email,
                        EmployeeRole.Employee,
                        PasswordHasher.Hash("Temp1234"),
                        user.Id
                    );
                    newEmployees.Add(newEmployee);
                }
                else
                {
                    if (employee.FullName != $"{user.FirstName} {user.LastName}" || employee.RedmineUserId != user.Id)
                    {
                        employee.Update($"{user.FirstName} {user.LastName}", user.Email, employee.Role);
                        employee.SetRedmineUserId(user.Id);
                        await _employeeRepository.UpdateAsync(employee);
                    }
                }
            }

            if (newEmployees.Any())
                await _employeeRepository.AddRangeAsync(newEmployees);

            var redmineIds = redmineUsers.Select(u => u.Id).ToHashSet();
            var toDeactivate = existingEmployeesList
                .Where(e => !redmineIds.Contains(e.RedmineUserId) && e.Role != EmployeeRole.Administrator) 
                .ToList();

            foreach (var emp in toDeactivate)
            {
                emp.ChangeStatus(false); //desactivar en lugar de borrar
                await _employeeRepository.UpdateAsync(emp);
            }

            await _unitOfWork.SaveChangesAsync(ct);

            return newEmployees.Count;
        }
    }
}
