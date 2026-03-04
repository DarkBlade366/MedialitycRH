using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Application.Features.Payrolls.Payments.Vacation.Queries;
using FluentValidation;

namespace Application.Features.Payrolls.Payments.Vacation.Validations
{
    public class GetVacationPaymentsPagedValidator : AbstractValidator<GetVacationPaymentsPagedQuery>
    {
        public GetVacationPaymentsPagedValidator()
        {
            RuleFor(x => x.Page)
                .GreaterThan(0).WithMessage("Page number must be greater than 0.");

            RuleFor(x => x.PageSize)
                .InclusiveBetween(1, 100).WithMessage("Page size must be between 1 and 100.");

            RuleFor(x => x)
                .Must(x => x.From!.Value <= x.To!.Value)
                .WithMessage("From date must be earlier than or equal to To date.")
                .When(x => x.From.HasValue && x.To.HasValue);
        }
    }
}
