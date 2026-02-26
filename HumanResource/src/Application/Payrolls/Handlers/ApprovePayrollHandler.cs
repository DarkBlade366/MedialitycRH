using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Application.Payrolls.Commands;
using Domain.Interfaces;

namespace Application.Payrolls.Handlers
{
    public class ApprovePayrollHandler
    {
        private readonly IPayrollRepository _payrollRepository;

        public ApprovePayrollHandler(IPayrollRepository payrollRepository)
        {
            _payrollRepository = payrollRepository;
        }

        public async Task Handle(ApprovePayrollCommand command)
        {
            var payroll = await _payrollRepository
                .GetByIdAsync(command.PayrollId)
                ?? throw new Exception("Payroll not found.");

            payroll.MarkUnderReview();
            payroll.Approve();

            await _payrollRepository.UpdateAsync(payroll);
        }
    }
}