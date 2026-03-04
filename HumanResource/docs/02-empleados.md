# 02 - Empleados

## Descripción General
Módulo de gestión de empleados. Los empleados pueden crearse manualmente o sincronizarse automáticamente desde Redmine. Cada empleado tiene un `RedmineUserId` que lo vincula con su usuario de Redmine.

## Endpoints

### POST /employees
**Acceso**: Administrator

**Descripción**: Crea un nuevo empleado manualmente.

**Request Body**:
```json
{
  "fullName": "Juan Pérez",
  "email": "juan.perez@gmail.com",
  "password": "XXXXXXXX",
  "redmineUserId": 100,
  "role": 0
}
```

**Roles disponibles (enum EmployeeRole)**:
- 1 = Employee
- 2 = Administrator
- 3 = HumanResources
- 4 = ProjectManager

**Response** (200 OK): `Guid` del empleado creado.

---

### GET /employees
**Acceso**: Administrator, HumanResources

**Descripción**: Lista empleados con paginación.

**Query Parameters**:
- `page` (int, requerido): Página actual.
- `pageSize` (int, requerido): Cantidad por página.

**Response** (200 OK):
```json
{
  "items": [
    {
      "id": "guid",
      "fullName": "Juan Pérez",
      "email": "juan@...",
      "role": 0,
      "isActive": true,
      "redmineUserId": 100
    }
  ],
  "totalCount": 50,
  "page": 1,
  "pageSize": 10
}
```

---

### GET /employees/{id:guid}
**Acceso**: Administrator, HumanResources

**Descripción**: Obtiene un empleado por su ID interno (Guid).

**Response** (200 OK): Detalle completo del empleado.

---

### GET /employees/redmine/{redmineUserId}
**Acceso**: Administrator, HumanResources

**Descripción**: Busca un empleado por su RedmineUserId.

**Path Parameter**: `redmineUserId` (int)

**Response** (200 OK): Detalle del empleado vinculado a ese usuario Redmine.

---

### PUT /employees/{id}/status
**Acceso**: Administrator

**Descripción**: Activa o desactiva un empleado (borrado lógico).

**Request Body**:
```json
{
  "isActive": false
}
```

**Response** (204 No Content)

---

### GET /employees/{EmployeeId}/vacation-balance
**Acceso**: Administrator, HumanResource

**Descripción**: Consulta el balance de vacaciones de un empleado.

**Response** (200 OK):
```json
{
  "employeeId": "guid",
  "accruedDays": 15.0,
  "usedDays": 5.0,
  "availableDays": 10.0
}
```

---

### POST /employees/{EmployeeId}/use-vacation
**Acceso**: Administrator, HumanResource, ProjectManager, Employee

**Descripción**: Registra uso de días de vacaciones.

**Request Body**:
```json
{
  "employeeId": "guid",
  "days": 3
}
```

**Response** (200 OK)

## Notas
- Los empleados sincronizados desde Redmine se crean con password temporal `Temp1234`.
- Al desactivar un empleado no se elimina de la BD, solo se marca `IsActive = false`.
