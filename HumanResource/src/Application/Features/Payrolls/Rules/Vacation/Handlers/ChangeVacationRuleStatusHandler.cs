using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Application.Common.Interfaces;
using Domain.Features.Payrolls.Interfaces;

namespace Application.Features.Payrolls.Rules.Vacation.Handlers
{
    public class ChangeVacationRuleStatusHandler
    {
        private readonly IVacationRuleRepository _repository;
        private readonly IUnitOfWork _unitOfWork;
    
        public ChangeVacationRuleStatusHandler(
            IVacationRuleRepository repository, 
            IUnitOfWork unitOfWork)
        {
            _repository = repository;
            _unitOfWork = unitOfWork;
        }
    }
}