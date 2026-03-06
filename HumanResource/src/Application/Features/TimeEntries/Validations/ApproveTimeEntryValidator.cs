using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FluentValidation;
using Application.Features.TimeEntries.Commands;

namespace Application.Features.TimeEntries.Validations
{
    public class ApproveTimeEntryValidator : AbstractValidator<ApproveTimeEntryCommand>
    {
        public ApproveTimeEntryValidator()
        {
            RuleFor(x => x.TimeEntryId)
                .NotEmpty()
                .WithMessage("TimeEntryId is required.");

            RuleFor(x => x.ApprovedHours)
                .GreaterThanOrEqualTo(0)
                .WithMessage("Approved hours must be greater or equal to zero.");
        }
    }
}