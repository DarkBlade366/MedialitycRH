using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Application.Auth.Commands;
using Application.Auth.DTOs;
using Application.Common.Security;
using Domain.Common.Security;
using Domain.Features.Employees.Interfaces;
namespace Application.Auth.Handlers
{
    public class LoginHandler
    {
        private readonly IEmployeeRepository _repo;
        private readonly ITokenGenerator _tokenGenerator;
        public LoginHandler(IEmployeeRepository repo, ITokenGenerator tokenGenerator)
        {
            _repo = repo;
            _tokenGenerator = tokenGenerator;
        }
        public async Task<LoginResponseDto> Handle(LoginCommand command)
        {
            var employee = await _repo.GetByEmailAsync(command.Email);
            if (employee == null || !employee.IsActive)
                throw new UnauthorizedAccessException("Invalid credentials");
            if (!PasswordHasher.Verify(command.Password, employee.PasswordHash))
                throw new UnauthorizedAccessException("Invalid credentials");
            // Generate JWT
            var token = _tokenGenerator.GenerateToken(
                employee.Id,
                employee.FullName,
                employee.Role.ToString()
            );
            return new LoginResponseDto
            {
                Token = token,
                EmployeeId = employee.Id,
                FullName = employee.FullName,
                Role = employee.Role.ToString()
            };
        }
    }
}