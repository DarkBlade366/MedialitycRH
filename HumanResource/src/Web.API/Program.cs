using System.Security.Claims;
using System.Text;  
using FastEndpoints;
using FastEndpoints.Swagger;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Infrastructure;
using Application;

var builder = WebApplication.CreateBuilder(args);

// Conexion con Infrastructure
builder.Services.AddInfrastructure(builder.Configuration);
//Conexion con Application
builder.Services.AddApplication();

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
            s.Version = "Only User";
        };
    });

builder.Services.AddEndpointsApiExplorer();

var app = builder.Build();

//await DatabaseSeeder.SeedAsync(app.Services);

app.UseAuthentication(); 
app.UseAuthorization();

app.UseFastEndpoints();

app.UseOpenApi();

app.UseSwaggerGen();

app.Run();
