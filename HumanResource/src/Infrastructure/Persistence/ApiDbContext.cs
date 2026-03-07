using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Domain.Common;
using Domain.Features.Payrolls.Rules;
using Domain.Features.Payrolls.Aggregates;
using Domain.Features.Payrolls.Aggregates.Payments;
using Domain.Features.Employees.Aggregates;
using Domain.Features.TimeEntries.Aggregates;
using Domain.Features.Projects.Aggregates;
using Domain.Features.Payrolls.Entities;
using Domain.Features.Employees.Entities;
namespace Infrastructure.Persistence
{
    public class ApiDbContext : DbContext
    {
        public ApiDbContext(DbContextOptions<ApiDbContext> options) : base(options)
        {

        }

        //DbSets
        public DbSet<Employee> Employees => Set<Employee>();
        public DbSet<EmployeeAguinaldoBalance> EmployeeAguinaldoBalances => Set<EmployeeAguinaldoBalance>();
        public DbSet<EmployeeVacationBalance> EmployeeVacationBalances => Set<EmployeeVacationBalance>();
        public DbSet<TimeEntry> TimeEntries => Set<TimeEntry>();
        public DbSet<Project> Projects => Set<Project>();
        public DbSet<ProjectMilestone> ProjectMilestones { get; set; }
        public DbSet<ProjectRule> ProjectRules => Set<ProjectRule>();
        public DbSet<ProjectPayment> ProjectPayments => Set<ProjectPayment>();
        public DbSet<Payroll> Payrolls => Set<Payroll>();
        public DbSet<PayrollComponent> PayrollComponents => Set<PayrollComponent>();
        public DbSet<BaseSalaryRule> BaseSalaryRules => Set<BaseSalaryRule>();
        public DbSet<VacationRule> VacationRules => Set<VacationRule>();
        public DbSet<VacationPayment> VacationPayments => Set<VacationPayment>();
        public DbSet<AguinaldoRule> AguinaldoRules => Set<AguinaldoRule>();
        public DbSet<AguinaldoPayment> AguinaldoPayments => Set<AguinaldoPayment>();
        public DbSet<MilestoneRule> MilestoneRules => Set<MilestoneRule>();
        public DbSet<MilestonePayment> MilestonePayments => Set<MilestonePayment>();
        public DbSet<MilestoneParticipation> MilestoneParticipations => Set<MilestoneParticipation>();
        public DbSet<DeductionRule> DeductionRules => Set<DeductionRule>();
        public DbSet<DeductionRule> Deductionpayments => Set<DeductionRule>();
        public DbSet<OvertimeRule> OvertimeRules => Set<OvertimeRule>();
        public DbSet<OvertimePayment> OvertimePayments => Set<OvertimePayment>();
        public DbSet<ProductivityRule> ProductivityRules => Set<ProductivityRule>();
        public DbSet<ProductivityPayment> ProductivityPayments => Set<ProductivityPayment>();
        public DbSet<ActivityProductivityWeight> ActivityProductivityWeights => Set<ActivityProductivityWeight>();

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