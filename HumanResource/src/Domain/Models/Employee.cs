using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Domain.Enums;

namespace Domain.Models
{
    public class Employee
    {
        public Guid Id { get; private set; }
        public string FullName { get; private set; }  = string.Empty;
        public string Email { get; private set; }  = string.Empty;
        public string PasswordHash { get; private set; } = string.Empty;
        public EmployeeRole Role { get; private set; }
        public bool IsActive { get; private set; }

        // Redmine (solo referencia por ahora)
        public string? RedmineUserId { get; private set; }  = string.Empty;
        protected Employee() { } 
        public Employee(string fullName, string email, EmployeeRole role) 
        { 
            Id = Guid.NewGuid(); 
            FullName = fullName; 
            Email = email; 
            Role = role; 
            IsActive = true; 
        } 
        public void Deactivate() 
        { 
            IsActive = false; 
        }
        public void SetPasswordHash(string passwordHash)
        {
            PasswordHash = passwordHash;
        }
    }
}