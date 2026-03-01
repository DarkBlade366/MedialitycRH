// using System.Threading;
// using System.Threading.Tasks;
// using FastEndpoints;
// using Application.Features.Payrolls.Commands;
// using Application.Features.Payrolls.Handlers;

// namespace Web.API.Endpoints.Payrolls
// {
//     public class ApprovePayrollEndpoint : EndpointWithoutRequest
//     {
//         private readonly ApprovePayrollHandler _handler;

//         public ApprovePayrollEndpoint(ApprovePayrollHandler handler)
//         {
//             _handler = handler;
//         }

//         public override void Configure()
//         {
//             Put("/payrolls/{PayrollId}/approve");
//             Roles("Administrator", "HumanResources");
//             Summary(s =>
//             {
//                 s.Summary = "Approve a payroll";
//                 s.Description = "Moves payroll from UnderReview to Approved state.";
//                 s.ExampleRequest = new ApprovePayrollCommand
//                 {
//                     PayrollId = Guid.Parse("00000000-0000-0000-0000-000000000000")
//                 };
//             });
//         }

//         public override async Task HandleAsync(CancellationToken ct)
//         {
//             var payrollId = Route<Guid>("PayrollId");

//             var req = new ApprovePayrollCommand
//             {
//                 PayrollId = payrollId
//             };

//             await _handler.Handle(req);
//             await Send.NoContentAsync(ct);
//         }
//     }
// }