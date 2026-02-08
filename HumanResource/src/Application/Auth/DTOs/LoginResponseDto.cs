using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Application.Auth.DTOs
{
    public class LoginResponseDto
    {
        public string Token { get; init; } = string.Empty;
        public Guid EmployeeId { get; init; }
        public string FullName { get; init; } = string.Empty;
        public string Role { get; init; } = string.Empty;
    }
}