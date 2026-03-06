using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FluentValidation;
using Application.Features.TimeEntries.Commands;

namespace Application.Features.TimeEntries.Validations
{
    public class ApproveTimeEntriesBatchValidator : AbstractValidator<ApproveTimeEntriesBatchCommand>
    {
        public ApproveTimeEntriesBatchValidator()
        {
            RuleFor(x => x.Items)
                .NotEmpty()
                .WithMessage("At least one time entry must be provided.");

            RuleForEach(x => x.Items).ChildRules(item =>
            {
                item.RuleFor(i => i.TimeEntryId)
                    .NotEmpty()
                    .WithMessage("TimeEntryId is required.");

                item.RuleFor(i => i.ApprovedHours)
                    .GreaterThanOrEqualTo(0)
                    .WithMessage("ApprovedHours must be >= 0.");
            });
        }
    }
}