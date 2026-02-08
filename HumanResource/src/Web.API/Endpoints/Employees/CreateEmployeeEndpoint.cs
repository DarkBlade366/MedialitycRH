using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Application.Employees.Commands;
using Application.Employees.Handlers;
using Application.Employees.Validations;
using FastEndpoints;

namespace Web.API.Endpoints.Employees
{
    public class CreateEmployeeEndpoint : Endpoint<CreateEmployeeCommand, Guid>
    {
        private readonly CreateEmployeeHandler _handler;

        public CreateEmployeeEndpoint(CreateEmployeeHandler handler)
        {
            _handler = handler;
        }

        public override void Configure()
        {
            Post("/employees");
            AllowAnonymous();  //luego se especifica
            Validator<CreateEmployeeValidation>();
            Summary(s =>
            {
                s.Summary = "Crear un nuevo empleado";
                s.Description = "Crea un nuevo empleado con los datos proporcionados.";
                s.ExampleRequest = new CreateEmployeeCommand
                {
                    FullName = "Juan Pérez",
                    Email = "juan.perez@gmail.com",
                    Password = "XXXXXXXX"
                };
            });
        }

        public override async Task HandleAsync(CreateEmployeeCommand req, CancellationToken ct)
        {
            var id = await _handler.Handle(req);
            await Send.OkAsync(id, ct);
        }
    }
}