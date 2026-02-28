using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Domain.Common;
using Domain.Features.Employees.Enums;
using Domain.Features.Employees.Entities;

namespace Domain.Features.Employees.Aggregates
{
    public class Employee : BaseEntity
    {
        public Guid Id { get; private set; }
        public string FullName { get; private set; } = string.Empty;
        public string Email { get; private set; } = string.Empty;
        public string PasswordHash { get; private set; } = string.Empty;
        public EmployeeRole Role { get; private set; }
        public bool IsActive { get; private set; }
        public int RedmineUserId { get; private set; }

        private EmployeeAguinaldoBalance _aguinaldoBalance = null!;
        private EmployeeVacationBalance _vacationBalance = null!;

        public EmployeeAguinaldoBalance AguinaldoBalance => _aguinaldoBalance;
        public EmployeeVacationBalance VacationBalance => _vacationBalance;

        protected Employee() { }

        public Employee(
            string fullName,
            string email,
            EmployeeRole role,
            string passwordHash,
            int redmineUserId)
        {
            if (string.IsNullOrWhiteSpace(fullName))
                throw new ArgumentException("Full name is required.");

            if (string.IsNullOrWhiteSpace(email))
                throw new ArgumentException("Email is required.");

            if (string.IsNullOrWhiteSpace(passwordHash))
                throw new ArgumentException("Password hash is required.");

            Id = Guid.NewGuid();
            FullName = fullName.Trim();
            Email = email.Trim().ToLowerInvariant();
            Role = role;
            PasswordHash = passwordHash;
            RedmineUserId = redmineUserId;
            IsActive = true;

            _aguinaldoBalance = new EmployeeAguinaldoBalance(Id);
            _vacationBalance = new EmployeeVacationBalance(Id);
        }  

        public void ChangeStatus(bool isActive)
        {
            if (IsActive == isActive)
                return;

            IsActive = isActive;
            MarkUpdated();
        }

        public void Update(string fullName, string email, EmployeeRole role)
        {
            if (string.IsNullOrWhiteSpace(fullName))
                throw new ArgumentException("Full name is required.");

            if (string.IsNullOrWhiteSpace(email))
                throw new ArgumentException("Email is required.");

            FullName = fullName.Trim();
            Email = email.Trim().ToLowerInvariant();
            Role = role;

            MarkUpdated();
        }

        public void SetRedmineUserId(int redmineUserId)
        {
            RedmineUserId = redmineUserId;
            MarkUpdated();
        }

        // 👇 MÉTODOS DE DOMINIO PARA BALANCES

        public void AccrueAguinaldo(decimal amount)
        {
            _aguinaldoBalance.Accrue(amount);
            MarkUpdated();
        }

        public decimal PayAguinaldo()
        {
            var paid = _aguinaldoBalance.Pay();
            MarkUpdated();
            return paid;
        }

        public void AccrueVacationDays(decimal days)
        {
            _vacationBalance.Accrue(days);
            MarkUpdated();
        }

        public void UseVacationDays(decimal days)
        {
            _vacationBalance.Use(days);
            MarkUpdated();
        }
    }
}