using System;
using System.Linq;
using Domain.Features.Payrolls.Aggregates;
using Domain.Features.Payrolls.Entities;
using Domain.Features.Payrolls.Enums;
using Domain.Features.Payrolls.Services.Context;
using Domain.Features.Payrolls.Services.Interfaces;
using Domain.Features.Projects.Enums;

namespace Domain.Features.Payrolls.Services.Calculators
{
    public class ProjectCalculator : IEarningCalculator
    {
        public void Calculate(Payroll payroll, PayrollCalculationContext context)
        {
            Console.WriteLine("[PROJECT] Starting project bonus calculation");

            foreach (var project in context.CompletedProjects)
            {
                Console.WriteLine($"[PROJECT] Evaluating project {project.RedmineProjectId}");

                if (payroll.ProjectPayments.Any(p =>
                        p.RedmineProjectId == project.RedmineProjectId))
                {
                    Console.WriteLine("[PROJECT] Already processed in this payroll");
                    continue;
                }

                var rule = context.ProjectRules
                    .FirstOrDefault(r =>
                        r.IsActive &&
                        r.RedmineProjectId == project.RedmineProjectId);

                var projectEntries = context.TimeEntries
                    .Where(t => t.RedmineProjectId == project.RedmineProjectId)
                    .ToList();

                var participants = projectEntries
                    .Select(t => t.EmployeeId)
                    .Distinct()
                    .ToList();

                Console.WriteLine($"[PROJECT] Participants found: {participants.Count}");

                if (participants.Count == 0)
                {
                    Console.WriteLine("[PROJECT] No participants found. Skipping.");
                    continue;
                }

                decimal amount = 0;

                if (rule == null)
                {
                    Console.WriteLine("[PROJECT] No rule configured. Registering payment = 0");
                }
                else
                {
                    amount = rule.BonusAmount / participants.Count;

                    Console.WriteLine($"[PROJECT] Total bonus: {rule.BonusAmount}");
                    Console.WriteLine($"[PROJECT] Individual amount: {amount}");

                    if (participants.Contains(payroll.EmployeeId))
                    {
                        payroll.AddComponent(new PayrollComponent(
                            PayrollComponentType.ProjectBonus,
                            PayrollComponentCategory.Earning,
                            $"Project Bonus - Project {project.RedmineProjectId}",
                            amount,
                            rule.Id));

                        Console.WriteLine("[PROJECT] Employee participated. Bonus added.");
                    }
                }

                payroll.AddProjectPayment(
                    project.RedmineProjectId,
                    amount,
                    DateTime.UtcNow);

                Console.WriteLine($"[PROJECT] Project payment registered: {amount}");
            }

            Console.WriteLine("[PROJECT] Project bonus calculation finished");
        }
    }
}