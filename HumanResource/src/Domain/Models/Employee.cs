using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Domain.Models
{
    public class Employee
    {
        public Guid Id { get; private set; }
        public string FullName { get; private set; }
        public string Email { get; private set; }
        public EmployeeRole Role { get; private set; }
        public bool IsActive { get; private set; }

        // Redmine (solo referencia por ahora)
        public string? RedmineUserId { get; private set; }
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
    }
}