using System.Security.Claims;
using System.Text;
using FastEndpoints;
using FastEndpoints.Swagger;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Infrastructure;
using Application;
using Infrastructure.Persistence;
using Infrastructure.Seed;
using Domain.Features.Payrolls.Services.Engines;

var builder = WebApplication.CreateBuilder(args);

// Conexion con Infrastructure
builder.Services.AddInfrastructure(builder.Configuration);
//Conexion con Application
builder.Services.AddApplication();
//Conexion con Domain
builder.Services.AddScoped<PayrollEngine>();

// Configuración JWT 
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = builder.Configuration["Jwt:Issuer"],
            ValidAudience = builder.Configuration["Jwt:Audience"],
            IssuerSigningKey = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"]!)),

            RoleClaimType = ClaimTypes.Role,
            NameClaimType = ClaimTypes.NameIdentifier
        };
    });

builder.Services.AddAuthorization();

// Configuración FaStEnpoint y Swagger 
builder.Services
    .AddFastEndpoints()
    .SwaggerDocument(o =>
    {
        o.DocumentSettings = s =>
        {
            s.Title = "Api Medialityc for Human Resource";
            s.Description = "API for Medialitic company for human resource";
        };
    });

builder.Services.AddEndpointsApiExplorer();

var app = builder.Build();
//await DatabaseSeeder.SeedAsync(app.Services);
using (var scope = app.Services.CreateScope())
{
    var dbContext = scope.ServiceProvider
        .GetRequiredService<ApiDbContext>();

    await dbContext.Database.MigrateAsync();
    await AdminSeeder.SeedAsync(dbContext);
}

app.UseAuthentication();
app.UseAuthorization();

app.UseFastEndpoints();

app.UseOpenApi();

app.UseSwaggerGen();

app.Run();
