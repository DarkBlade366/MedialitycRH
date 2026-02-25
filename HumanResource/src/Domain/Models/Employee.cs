using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Domain.Enums;
using Domain.Common;

namespace Domain.Models
{
    public class Employee : BaseEntity
    {
        public Guid Id { get; private set; }
        public string FullName { get; private set; }  = string.Empty;
        public string Email { get; private set; }  = string.Empty;
        public string PasswordHash { get; private set; } = string.Empty;
        public EmployeeRole Role { get; private set; }
        public bool IsActive { get; private set; }

        public int RedmineUserId { get; private set; }

        protected Employee() { } // EF Core

        public Employee(string fullName, string email, EmployeeRole role, string passwordHash, int redmineUserId)
        { 
            Id = Guid.NewGuid(); 
            FullName = fullName; 
            Email = email; 
            Role = role; 
            RedmineUserId = redmineUserId;
            IsActive = true; 
            PasswordHash = passwordHash;
        } 
        
        public void ChangeStatus(bool isActive)
        {
            if (IsActive == isActive)
                return;
        
            IsActive = isActive;
        }
        public void Update(string fullName, string email, EmployeeRole role)
        {
            FullName = fullName;
            Email = email;
            Role = role;
        }
        
        public void SetRedmineUserId(int redmineUserId)
        {
            RedmineUserId = redmineUserId;
        }
    }
}