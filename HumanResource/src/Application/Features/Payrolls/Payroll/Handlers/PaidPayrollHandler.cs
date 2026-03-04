using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Application.Common.Interfaces;
using Application.Features.Payrolls.Payroll.Commands;
using Application.Features.Payrolls.Payroll.DTOs;
using Domain.Features.Payrolls.Interfaces;

namespace Application.Features.Payrolls.Payroll.Handlers
{
    public class PaidPayrollHandler
    {
        private readonly IPayrollRepository _repository;
        private readonly IUnitOfWork _unitOfWork;
    
        public PaidPayrollHandler(
            IPayrollRepository repository, 
            IUnitOfWork unitOfWork)
        {
            _repository = repository;
            _unitOfWork = unitOfWork;
        }

        public async Task<PayrollResponse> Handle(
            PaidPayrollCommand command,
            CancellationToken cancellationToken)
        {
            var payroll = await _repository.GetByIdAsync(command.Id);

            if(payroll == null)
                throw new Exception("Payroll not found.");

            payroll.MarkAsPaid();

            await _unitOfWork.SaveChangesAsync();

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