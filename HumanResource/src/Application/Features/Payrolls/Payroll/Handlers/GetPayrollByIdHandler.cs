using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Application.Features.Payrolls.Payroll.DTOs;
using Application.Features.Payrolls.Payroll.Queries;
using Domain.Features.Payrolls.Interfaces;

namespace Application.Features.Payrolls.Payroll.Handlers
{
    public class GetPayrollByIdHandler
    {
        private readonly IPayrollRepository _repository;
    
        public GetPayrollByIdHandler(IPayrollRepository repository)
        {
            _repository = repository;
        }
    
        public async Task<PayrollResponse?> HandleAsync(GetPayrollByIdQuery query)
        {
            var payroll = await _repository.GetByIdAsync(query.Id);
    
            if (payroll is null)
                throw new Exception("Payroll not found.");
    
            return new PayrollResponse
            {
                Id = payroll.Id,
                EmployeeId = payroll.EmployeeId,
                PeriodStart = payroll.PeriodStart,
                PeriodEnd = payroll.PeriodEnd,
                GrossAmount = payroll.GrossAmount,
                TotalDeductions = payroll.TotalDeductions,
                NetAmount = payroll.NetAmount,
                Status = payroll.Status.ToString(),
                Components = payroll.Components
                    .Select(c => new PayrollComponentResponse
                    {
                        Type = c.Type.ToString(),
                        Category = c.Category.ToString(),
                        Description = c.Description,
                        Amount = c.Amount
                    })
                    .ToList()
            };
        }
    }
}