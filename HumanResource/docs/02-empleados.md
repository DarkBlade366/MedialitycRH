# 02 - Empleados

## Descripción General

Módulo de gestión de empleados. Los empleados pueden crearse manualmente o sincronizarse automáticamente desde Redmine. Cada empleado tiene un RedmineUserId que lo vincula con su usuario de Redmine.

## Endpoints

### POST /employees

**Acceso**: Administrator

**Descripción**: Crea un nuevo empleado manualmente.

**Request Body**:
```json
{
  "fullName": "Juan Pérez",
  "email": "juan.perez@gmail.com",
  "password": "MiClaveSegura123",
  "redmineUserId": 100,
  "role": 0
}
```

**Roles disponibles (enum EmployeeRole)**:
- 1 = Employee
- 2 = Administrator
- 3 = HumanResources
- 4 = ProjectManager

**Response (200 OK)**: Guid del empleado creado.

### GET /employees

**Acceso**: Administrator, HumanResources

**Descripción**: Lista empleados con paginación.

**Query Parameters**:
- `page` (int, requerido): Número de página.
- `pageSize` (int, requerido): Cantidad de elementos por página (máx. 100).

**Response (200 OK)**:
```json
{
  "items": [
    {
      "id": "3fa85f64-5717-4562-b3fc-2c963f66afa6",
      "fullName": "Juan Pérez",
      "email": "juan.perez@gmail.com",
      "role": "Employee",
      "isActive": true,
      "redmineUserId": 100
    }
  ],
  "totalCount": 50,
  "page": 1,
  "pageSize": 10
}
```

### GET /employees/{id:guid}

**Acceso**: Administrator, HumanResources

**Descripción**: Obtiene un empleado por su ID interno (Guid).

**Path Parameter**: id (Guid)

**Response (200 OK)**:
```json
{
  "id": "3fa85f64-5717-4562-b3fc-2c963f66afa6",
  "fullName": "Juan Pérez",
  "email": "juan.perez@gmail.com",
  "role": "Employee",
  "isActive": true,
  "redmineUserId": 100,
  "vacationDaysAvailable": 12.5,
  "aguinaldoAvailable": 1250.75,
  "createdAt": "2026-01-01T00:00:00Z",
  "updatedAt": null
}
```

### GET /employees/redmine/{redmineUserId}

**Acceso**: Administrator, HumanResources

**Descripción**: Busca un empleado por su RedmineUserId.

**Path Parameter**: redmineUserId (int)

**Response (200 OK)**: Mismo formato que GET /employees/{id}.

### PUT /employees/{id}/status

**Acceso**: Administrator

**Descripción**: Activa o desactiva un empleado (borrado lógico).

**Path Parameter**: id (Guid)

**Request Body**:
```json
{
  "isActive": false
}
```

**Response (204 No Content)**

### GET /employees/{employeeId}/vacation-balance

**Acceso**: Administrator, HumanResources

**Descripción**: Consulta el balance de vacaciones de un empleado.

**Path Parameter**: employeeId (Guid)

**Response (200 OK)**:
```json
{
  "employeeId": "3fa85f64-5717-4562-b3fc-2c963f66afa6",
  "accruedDays": 15.0,
  "usedDays": 5.0,
  "availableDays": 10.0
}
```

### POST /employees/{employeeId}/use-vacation

**Acceso**: Administrator, HumanResources, ProjectManager, Employee

**Descripción**: Registra uso de días de vacaciones.

**Path Parameter**: employeeId (Guid)

**Request Body**:
```json
{
  "employeeId": "3fa85f64-5717-4562-b3fc-2c963f66afa6",
  "days": 3
}
```

**Response (200 OK)**

## Notas

- Los empleados sincronizados desde Redmine se crean con password temporal `Temp1234`. Se recomienda que el empleado cambie su contraseña en el primer inicio de sesión (el endpoint de cambio de contraseña no está incluido en esta versión).

- Al desactivar un empleado no se elimina de la base de datos, solo se marca `IsActive = false`. Esto impide nuevos inicios de sesión y no se incluirá en procesos de nómina.

- Los balances de vacaciones se actualizan automáticamente mediante un background service que ejecuta la acumulación mensual según la regla activa.
