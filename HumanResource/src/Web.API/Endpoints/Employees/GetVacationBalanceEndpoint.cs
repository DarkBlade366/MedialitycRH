using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Application.Features.Employees.DTOs;
using Application.Features.Employees.Handlers;
using Application.Features.Employees.Queries;
using Application.Features.Employees.Validations;
using FastEndpoints;

namespace Web.API.Endpoints.Employees
{
    public class GetVacationBalanceEndpoint : Endpoint<GetVacationBalanceQuery, VacationBalanceResponse>
    {
        private readonly GetVacationBalanceHandler _handler;

        public GetVacationBalanceEndpoint(GetVacationBalanceHandler handler)
        {
            _handler = handler;
        }
        public override void Configure()
        {
            Get("/employees/{EmployeeId}/vacation-balance");
            Roles("Administrator, HumanResource");
            Validator<GetVacationBalanceQueryValidator>();
            Summary(s =>
            {
                s.Summary = "Get vacation balance for an employee";
                s.Description = "Returns accrued, used, and available vacation days for a specific employee.";
                s.ExampleRequest = new 
                { 
                    EmployeeId = Guid.Parse("00000000-0000-0000-0000-000000000000") 
                };
            });
        }
    
        public override async Task HandleAsync(GetVacationBalanceQuery req, CancellationToken ct)
        {
            var result = await _handler.Handle(req, ct);
            await Send.OkAsync(result, ct);
        }
    }
}