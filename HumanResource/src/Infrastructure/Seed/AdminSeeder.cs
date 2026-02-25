using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Domain.Models;
using Microsoft.EntityFrameworkCore;
using Infrastructure.Persistence;
using Domain.Enums;
using Domain.Common.Security;

namespace Infrastructure.Seed
{
    public static class AdminSeeder 
    {
        public static async Task SeedAsync(ApiDbContext context)
        {
            var adminExists = await context.Employees
                .AnyAsync(e => e.Role == EmployeeRole.Administrator);

            if (adminExists)
                return;

            var admin = new Employee(
                redmineUserId: 0, // Asignar un ID de Redmine válido si es necesario
                fullName: "System Administrator",
                email: "admin@system.local",
                role: EmployeeRole.Administrator,
                passwordHash: PasswordHasher.Hash("Admin123")
            );

            context.Employees.Add(admin);
            await context.SaveChangesAsync();
        }
    }
}