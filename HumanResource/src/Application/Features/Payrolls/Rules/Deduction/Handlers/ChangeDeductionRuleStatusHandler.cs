using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Application.Common.Interfaces;
using Domain.Features.Payrolls.Interfaces;

namespace Application.Features.Payrolls.Rules.Deduction.Handlers
{
    public class ChangeDeductionRuleStatusHandler
    {
        private readonly IDeductionRuleRepository _repository;
        private readonly IUnitOfWork _unitOfWork;
    
        public ChangeDeductionRuleStatusHandler(
            IDeductionRuleRepository repository, 
            IUnitOfWork unitOfWork)
        {
            _repository = repository;
            _unitOfWork = unitOfWork;
        }
    }
}