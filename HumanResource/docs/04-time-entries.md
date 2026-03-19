# 04 - Time Entries

## Descripción General

Los time entries son las horas registradas en Redmine que se sincronizan con el sistema. Pueden ser aprobadas (total o parcialmente) por usuarios con permisos. Las horas aprobadas son las que se consideran para el cálculo de nóminas.

## Endpoints

### GET /time-entries

**Acceso**: Administrator, HumanResources, ProjectManager

**Descripción**: Lista time entries con paginación y filtros.

**Query Parameters**:
- `employeeId` (Guid, opcional): Filtrar por empleado.
- `from` (DateTime, opcional): Fecha de inicio (UTC).
- `to` (DateTime, opcional): Fecha de fin (UTC).
- `page` (int, requerido): Número de página.
- `pageSize` (int, requerido): Elementos por página.

**Response (200 OK)**:
```json
{
  "items": [
    {
      "id": "guid",
      "redmineTimeEntryId": 1001,
      "redmineProjectId": 123,
      "redmineActivityId": 10,
      "activityName": "Development",
      "employeeId": "guid",
      "hours": 8.5,
      "approvedHours": null,
      "reviewed": false,
      "spentOn": "2026-03-15T00:00:00Z"
    }
  ],
  "totalCount": 45,
  "page": 1,
  "pageSize": 10
}
```

### PUT /time-entries/approve

**Acceso**: Administrator, HumanResources, ProjectManager

**Descripción**: Aprueba un time entry individual.

**Request Body**:
```json
{
  "timeEntryId": "guid",
  "approvedHours": 8.0
}
```

**Response (200 OK)**: El time entry actualizado (mismo formato que en el listado).

### PUT /time-entries/approve-batch

**Acceso**: Administrator, HumanResources, ProjectManager

**Descripción**: Aprueba múltiples time entries en una sola petición.

**Request Body**:
```json
{
  "items": [
    {
      "timeEntryId": "guid1",
      "approvedHours": 8.0
    },
    {
      "timeEntryId": "guid2",
      "approvedHours": 6.5
    }
  ]
}
```

**Response (200 OK)**:
```json
[
  {
    "timeEntryId": "guid1",
    "success": true,
    "message": "Approved 8.0h of 8.5h",
    "timeEntry": { ... }
  },
  {
    "timeEntryId": "guid2",
    "success": false,
    "message": "Time entry already reviewed."
  }
]
```

## Notas

- Solo los time entries aprobados y con `approvedHours > 0` se consideran en el cálculo de horas trabajadas para nóminas.
- Una vez aprobado, no se puede modificar (el sistema rechazará nuevas aprobaciones).
- Los time entries no aprobados no afectan la nómina.
