using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Domain.Models;
using Microsoft.EntityFrameworkCore;
using Domain.Common;
namespace Infrastructure.Persistence
{
    public class ApiDbContext : DbContext
    {
        public ApiDbContext(DbContextOptions<ApiDbContext> options) : base(options)
        {
            
        }

        //DbSets
        public DbSet<Employee> Employees => Set<Employee>();
        public DbSet<TimeEntry> TimeEntries => Set<TimeEntry>();
        public DbSet<Project> Projects => Set<Project>();
        public DbSet<Payroll> Payrolls => Set<Payroll>();
        public DbSet<PayrollLine> PayrollLines => Set<PayrollLine>();
        public DbSet<PayrollComponent> PayrollComponents => Set<PayrollComponent>();
        public DbSet<SalaryConfiguration> SalaryConfigurations => Set<SalaryConfiguration>();
        public DbSet<ProjectBonusConfiguration> ProjectBonusConfigurations => Set<ProjectBonusConfiguration>();

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.ApplyConfigurationsFromAssembly(typeof(ApiDbContext).Assembly);

        }

        public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            foreach (var entry in ChangeTracker.Entries<BaseEntity>())
            {
                if (entry.State == EntityState.Modified)
                {
                    entry.Entity.MarkUpdated();
                }
            }

            return base.SaveChangesAsync(cancellationToken);
        }

    }
}