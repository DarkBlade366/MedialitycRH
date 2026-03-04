using System;
using System.Linq;
using Domain.Features.Payrolls.Aggregates;
using Domain.Features.Payrolls.Services.Context;
using Domain.Features.Payrolls.Services.Interfaces;
using Domain.Features.Payrolls.Enums;
using Domain.Features.Payrolls.Entities;

namespace Domain.Features.Payrolls.Services.Calculators
{
    public class MilestoneCalculator : IEarningCalculator
    {
        public void Calculate(Payroll payroll, PayrollCalculationContext context)
        {
            Console.WriteLine($"[MILESTONE] Iniciando cálculo de milestone bonus");

            var completedMilestones = context.ProjectMilestones
                .Where(m => m.CompletedAt.HasValue)
                .ToList();

            Console.WriteLine($"[MILESTONE] Milestones completados encontrados: {completedMilestones.Count}");

            foreach (var milestone in completedMilestones)
            {
                Console.WriteLine($"[MILESTONE] Evaluando milestone: {milestone.Name} - Proyecto {milestone.RedmineProjectId}");

                var rule = context.MilestoneRules
                    .FirstOrDefault(r =>
                        r.IsActive &&
                        r.RedmineProjectId == milestone.RedmineProjectId &&
                        r.MilestoneName == milestone.Name);

                if (rule == null)
                {
                    Console.WriteLine($"[MILESTONE] No se encontró regla activa para este milestone");
                    continue;
                }

                if (payroll.IsMilestonePaid(rule.Id))
                {
                    Console.WriteLine($"[MILESTONE] Milestone ya pagado anteriormente. RuleId: {rule.Id}");
                    continue;
                }

                var totalParticipants = milestone.Participations.Count;

                if (totalParticipants == 0)
                {
                    Console.WriteLine($"[MILESTONE] Milestone sin participantes. Se omite.");
                    continue;
                }

                var individualAmount = rule.BonusAmount / totalParticipants;

                Console.WriteLine($"[MILESTONE] Regla encontrada. Bonus total: {rule.BonusAmount}");
                Console.WriteLine($"[MILESTONE] Participantes: {totalParticipants}");
                Console.WriteLine($"[MILESTONE] Monto individual calculado: {individualAmount}");

                foreach (var participant in milestone.Participations)
                {
                    if (participant.EmployeeId != payroll.EmployeeId)
                        continue;

                    Console.WriteLine($"[MILESTONE] Empleado {payroll.EmployeeId} participa en milestone. Agregando bono.");

                    payroll.AddComponent(new PayrollComponent(
                        PayrollComponentType.MilestoneBonus,
                        PayrollComponentCategory.Earning,
                        $"Milestone Bonus - {rule.MilestoneName} (1/{totalParticipants})",
                        individualAmount,
                        rule.Id));
                    
                    participant.MarkAsPaid(); 

                    payroll.AddMilestonePayment(rule.Id, individualAmount, DateTime.UtcNow);

                    Console.WriteLine($"[MILESTONE] Pago registrado por {individualAmount}");
                }
            }

            Console.WriteLine($"[MILESTONE] Finalizó cálculo de milestone bonus");
        }
    }
}