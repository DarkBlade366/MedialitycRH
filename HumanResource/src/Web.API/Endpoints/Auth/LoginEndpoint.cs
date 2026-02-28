using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Application.Auth.Commands;
using Application.Auth.DTOs;
using Application.Auth.Handlers;
using Application.Auth.Validations;
using FastEndpoints;

namespace Web.API.Endpoints.Auth
{
    public class LoginEndpoint : Endpoint<LoginCommand, LoginResponseDto>
    {
        private readonly LoginHandler _handler;

        public LoginEndpoint(LoginHandler handler)
        {
            _handler = handler;
        }
        public override void Configure()
        {
            Post("/auth/login");
            AllowAnonymous(); //luego se especifica
            Validator<LoginValidation>();
            Summary(s =>
            {
                s.Summary = "Login.";
                s.Description = "Logs in with the provided credentials.";
                s.ExampleRequest = new LoginCommand
                {
                    Email = "admin@system.local",
                    Password = "Admin123"
                };
            });
        }

        public override async Task HandleAsync(LoginCommand command, CancellationToken ct)
        {
            var response = await _handler.Handle(command);
            await Send.OkAsync(response, ct);
        }
    }
}