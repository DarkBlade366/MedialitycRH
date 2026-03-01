// using System;
// using System.Collections.Generic;
// using System.Linq;
// using System.Threading.Tasks;
// using FastEndpoints;
// using Domain.Features.Payrolls.Interfaces;
// using Infrastructure.Reports;
// using Application.Features.Payrolls.Queries;
// using Application.Features.Payrolls.Handlers;

// namespace Web.API.Endpoints.Payrolls
// {
//     public class GetPayrollPdfEndpoint : EndpointWithoutRequest
//     {
//         private readonly GetPayrollPdfHandler _handler;

//         public GetPayrollPdfEndpoint(GetPayrollPdfHandler handler)
//         {
//             _handler = handler;
//         }

//         public override void Configure()
//         {
//             Get("/payrolls/{id:guid}/pdf");
//             Roles("Administrator", "HumanResources", "Employee");
//             Summary(s =>
//             {
//                 s.Summary = "Download payroll as PDF";
//                 s.Description = "Generates and downloads a payroll receipt in PDF format.";
//             });
//         }

//         public override async Task HandleAsync(CancellationToken ct)
//         {
//             var payrollId = Route<Guid>("id");

//             var result = await _handler.HandleAsync(
//                 new GetPayrollPdfQuery { PayrollId = payrollId }, ct);

//             await Send.BytesAsync(result.FileBytes!, "application/pdf", result.FileName, cancellation: ct);
//         }
//     }
// }