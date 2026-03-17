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
    private readonly ICacheService _cache;

    public SyncRedmineUsersHandler(IRedmineService redmineService, 
        IEmployeeRepository employeeRepository,
        IUnitOfWork unitOfWork,
        ICacheService cache)
    {
        _redmineService = redmineService;
        _employeeRepository = employeeRepository;
        _unitOfWork = unitOfWork;
        _cache = cache;
    }
        public async Task<int> Handle(CancellationToken ct)
        {
            var redmineUsers = await _redmineService.GetUsersAsync();

            if (!redmineUsers.Any())
                return 0;

            var redmineIds = redmineUsers
                .Select(u => u.Id)
                .ToHashSet();

            var existingEmployees = await _employeeRepository.GetAllActiveAsync();

            var employeeDictionary = existingEmployees
                .ToDictionary(e => e.RedmineUserId);

            var newEmployees = new List<Employee>();

            foreach (var user in redmineUsers)
            {
                if (string.IsNullOrWhiteSpace(user.Email))
                    continue;

                if (employeeDictionary.TryGetValue(user.Id, out var employee))
                {
                    var fullName = $"{user.FirstName} {user.LastName}";

                    if (employee.FullName != fullName)
                    {
                        employee.Update(fullName, user.Email, employee.Role);
                        employee.SetRedmineUserId(user.Id);
                    }
                }
                else
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
            }

            if (newEmployees.Any())
                await _employeeRepository.AddRangeAsync(newEmployees);

            var toDeactivate = existingEmployees
                .Where(e => !redmineIds.Contains(e.RedmineUserId)
                        && e.Role != EmployeeRole.Administrator)
                .ToList();

            foreach (var emp in toDeactivate)
            {
                emp.ChangeStatus(false);
            }

            await _unitOfWork.SaveChangesAsync(ct);
            
            await _cache.RemoveAsync("employees:all", ct);

            return newEmployees.Count;
        }
    }
}
