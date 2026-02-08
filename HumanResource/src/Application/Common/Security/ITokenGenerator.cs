using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Application.Common.Security
{
    public interface ITokenGenerator
    {
        string GenerateToken(
            Guid userId,
            string fullName,
            string role
        );
    }
}