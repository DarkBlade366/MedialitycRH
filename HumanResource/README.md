# Human Resource API

REST API built with .NET 9 and FastEndpoints following Clean Architecture principles.  
The system manages Employees with role-based authorization and basic Redmine synchronization.

---

# Overview

This project provides a RESTful API to manage employees within an organization.

Main features:

- Employee CRUD operations
- Pagination support
- Logical activation/deactivation
- Basic Redmine User ID synchronization
- JWT authentication
- Role-based authorization (Administrator)

The architecture ensures proper separation of concerns using Domain-Driven Design concepts.

---

# Architecture

The solution follows Clean Architecture and is divided into four main layers:

1. Domain  
   - Entities  
   - Interfaces  
   - Business rules  
   - Encapsulated behavior  

2. Application  
   - Commands  
   - Queries  
   - Handlers  
   - DTOs  
   - Business orchestration  

3. Infrastructure  
   - Entity Framework Core  
   - Repository implementations  
   - External services integration  
   - Database configuration  

4. Web.API  
   - FastEndpoints endpoints  
   - Validation  
   - JWT configuration  
   - Swagger documentation  

Dependency flow:

Web.API → Application → Domain  
Infrastructure → Domain  

---

# Authentication

Authentication is handled using JWT (JSON Web Tokens).

## Login

POST /auth/login

Request:

```json
{
  "email": "admin@company.com",
  "password": "Admin123*"
}
```

Response:

```json
{
  "token": "JWT_TOKEN"
}
```

The token contains:

- NameIdentifier (User Id)
- Name
- Role
- Expiration
- Issuer
- Audience

---

# Employee Module

All employee management endpoints require the Administrator role.

---

## Get All Employees (Paged)

GET /employees?page=1&pageSize=10

Returns a paginated list of employees.

---

## Get Employee By Id

GET /employees/{id}

Returns details of a specific employee.

---

## Create Employee

POST /employees

Creates a new employee.

---

## Update Employee

PUT /employees/{id}

Updates employee information.

---

## Change Employee Status

PUT /employees/{id}/status

Request:

```json
{
  "isActive": true
}
```

This endpoint allows activating or deactivating an employee.  
The status is changed through domain behavior to preserve encapsulation.

---

## Set Redmine User ID

PUT /employees/{id}/redmine

Request:

```json
{
  "redmineUserId": 12345
}
```

This endpoint assigns or updates the Redmine user ID associated with an employee.

Current implementation stores the Redmine ID locally without validating it against the Redmine API.

---

# Redmine Integration

The current implementation performs basic synchronization by storing a Redmine user ID.

A more advanced integration could include:

- Validating user existence through Redmine REST API
- Background synchronization service
- Event-driven updates (RabbitMQ)
- Automatic user data synchronization

These improvements are considered future enhancements.

---

# Technologies Used

- .NET 9
- FastEndpoints
- Entity Framework Core
- PostgreSQL
- JWT Authentication
- FluentValidation
- Clean Architecture
- CQRS (Commands & Queries separation)

---

# Pagination

Pagination is implemented for listing endpoints.

Example:

GET /employees?page=1&pageSize=10

The response includes:

- Items
- Total count
- Page number
- Page size

---

# Running the Project

1. Configure the database connection in appsettings.json.

2. Apply migrations:

```
dotnet ef database update
```

3. Run the project:

```
dotnet run
```

Swagger will be available at:

https://localhost:{port}/swagger

---

# Roles

Currently supported roles:

- Administrator

Authorization is enforced at the endpoint level.

---

# Design Decisions

- Clean Architecture for maintainability.
- Repository pattern for data abstraction.
- Domain encapsulation to avoid anemic entities.
- CQRS pattern for separation of read/write operations.
- JWT-based stateless authentication.
- Logical deletion (IsActive) instead of physical removal.

---

# Future Improvements

- Full Redmine API validation
- Background synchronization service
- Unit testing layer
- Docker support
- CI/CD integration
- Role expansion
- Audit logging

---

# License

This project is intended for educational and professional training purposes.
