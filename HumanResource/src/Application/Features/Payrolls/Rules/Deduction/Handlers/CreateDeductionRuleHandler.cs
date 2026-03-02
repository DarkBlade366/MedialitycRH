using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Application.Common.Interfaces;
using Domain.Features.Payrolls.Interfaces;

namespace Application.Features.Payrolls.Rules.Deduction.Handlers
{
    public class CreateDeductionRuleHandler
    {
        private readonly IDeductionRuleRepository _repository;
        private readonly IUnitOfWork _unitOfWork;
    
        public CreateDeductionRuleHandler(
            IDeductionRuleRepository repository, 
            IUnitOfWork unitOfWork)
        {
            _repository = repository;
            _unitOfWork = unitOfWork;
        }

        // var existingActive = (await _repository.GetAllAsync())
        //         .Any(r => r.IsActive);
        
        //     if (existingActive)
        //         throw new Exception("There is already an active deduction rule.");
    }
}