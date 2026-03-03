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
            // Agrupamos los milestones completados por proyecto y nombre
            var completedMilestones = context.ProjectMilestones
                .Where(m => m.CompletedAt.HasValue)
                .ToList();

            foreach (var milestone in completedMilestones)
            {
                // Obtenemos la regla activa que corresponda
                var rule = context.MilestoneRules
                    .FirstOrDefault(r =>
                        r.IsActive &&
                        r.RedmineProjectId == milestone.RedmineProjectId &&
                        r.MilestoneName == milestone.Name);

                if (rule == null)
                    continue;

                // Evitamos duplicados en la nómina
                if (payroll.IsMilestonePaid(rule.Id))
                    continue;

                var totalParticipants = milestone.Participations.Count;

                if (totalParticipants == 0)
                    continue;

                var individualAmount = rule.BonusAmount / totalParticipants;

                // Creamos un pago por cada participante
                foreach (var participant in milestone.Participations)
                {
                    if (participant.EmployeeId != payroll.EmployeeId)
                        continue;

                    // Agregamos al payroll como componente
                    payroll.AddComponent(new PayrollComponent(
                        PayrollComponentType.MilestoneBonus,
                        PayrollComponentCategory.Earning,
                        $"Milestone Bonus - {rule.MilestoneName} (1/{totalParticipants})",
                        individualAmount,
                        rule.Id));

                    // Marcamos que ya se aplicó esta regla en la nómina
                    payroll.AddMilestonePayment(rule.Id, individualAmount, DateTime.UtcNow);
                }
            }
        }
    }
}