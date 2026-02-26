using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FluentValidation;
using Domain.Enums;
using Application.SalaryConfigurations.Commands;

namespace Application.SalaryConfigurations.Validations
{
    public class UpdateSalaryConfigurationValidator : AbstractValidator<UpdateSalaryConfigurationCommand>
    {
        public UpdateSalaryConfigurationValidator()
        {
            RuleFor(x => x.Role)
                .IsInEnum()
                .WithMessage("Invalid employee role.");

            RuleFor(x => x.NewHourlyRate)
                .GreaterThanOrEqualTo(0)
                .WithMessage("New hourly rate must be non-negative.");
        }
    }
}