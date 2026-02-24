using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Domain.Enums;
using Domain.Interfaces;
using Domain.Models;

namespace Application.Redmine
{
    public class SyncRedmineUsersHandler
    {
        private readonly IRedmineService _redmineService;
        private readonly IEmployeeRepository _employeeRepository;

        public SyncRedmineUsersHandler(
            IRedmineService redmineService,
            IEmployeeRepository employeeRepository)
        {
            _redmineService = redmineService;
            _employeeRepository = employeeRepository;
        }

        public async Task<int> Handle()
        {
            var redmineUsers = await _redmineService.GetUsersAsync();

            if (!redmineUsers.Any())
                return 0;

            var emails = redmineUsers
                .Where(x => !string.IsNullOrWhiteSpace(x.Email))
                .Select(x => x.Email!)
                .ToList();

            var existingEmails = await _employeeRepository.GetExistingEmailsAsync(emails);
            var existingEmailsSet = existingEmails.ToHashSet();

            var newEmployees = new List<Employee>();

            foreach (var user in redmineUsers)
            {
                if (string.IsNullOrWhiteSpace(user.Email))
                    continue;

                if (existingEmailsSet.Contains(user.Email))
                    continue;

                var employee = new Employee(
                    $"{user.FirstName} {user.LastName}",
                    user.Email,
                    EmployeeRole.Employee,
                    BCrypt.Net.BCrypt.HashPassword("Temp1234!")
                );

                employee.SetRedmineUserId(user.Id);

                newEmployees.Add(employee);
            }

            if (newEmployees.Any())
                await _employeeRepository.AddRangeAsync(newEmployees);

            return newEmployees.Count;
        }
    }
}
