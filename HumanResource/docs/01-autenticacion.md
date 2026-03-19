# 01 - Autenticación

## Descripción General

El sistema utiliza JWT (JSON Web Tokens) para autenticación. Al iniciar sesión se obtiene un token que debe incluirse en el header `Authorization: Bearer {token}` de todas las peticiones protegidas.

## Roles del Sistema

- **Administrator**: Acceso total al sistema.
- **HumanResources**: Gestión de empleados, nóminas y reportes.
- **ProjectManager**: Visualización de proyectos, hitos y time entries.
- **Employee**: Acceso limitado a sus propios datos y vacaciones.

## Endpoints

### POST /auth/login

**Acceso**: Público (AllowAnonymous)

**Descripción**: Autentica un usuario y retorna un token JWT.

**Request Body**:
```json
{
  "email": "admin@system.local",
  "password": "Admin123"
}
```

**Response (200 OK)**:
```json
{
  "token": "eyJhbGciOiJIUzI1NiIs...",
  "employeeId": "3fa85f64-5717-4562-b3fc-2c963f66afa6",
  "fullName": "System Administrator",
  "role": "Administrator"
}
```

**Validaciones**:
- Email requerido y formato válido.
- Password requerido y mínimo 8 caracteres.

## Configuración JWT (appsettings.json)

```json
{
  "Jwt": {
    "Key": "TU_SECRET_KEY_DE_AL_MENOS_32_CARACTERES",
    "Issuer": "Payments.Api",
    "Audience": "Payments.Users",
    "AccessTokenMinutes": 60
  }
}
```

## Uso del Token

Incluir en todas las peticiones protegidas:
```
Authorization: Bearer eyJhbGciOiJIUzI1NiIs...
```

## Seed Inicial

Al iniciar la aplicación por primera vez se crea automáticamente un usuario administrador a través del AdminSeeder con las credenciales:

- **Email**: admin@system.local
- **Password**: Admin123

Se recomienda cambiar esta contraseña inmediatamente después del primer inicio de sesión.
