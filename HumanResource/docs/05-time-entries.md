# 05 - Time Entries (Horas Registradas)

## Descripción General
Los time entries representan las horas registradas por los empleados en Redmine. Se sincronizan automáticamente (ver doc 03) y se almacenan localmente. Son la base para el cálculo de horas extras y productividad en la nómina.

Cada time entry contiene:
- `RedmineTimeEntryId`: ID original en Redmine.
- `RedmineProjectId`: Proyecto Redmine al que pertenece.
- `EmployeeId`: Empleado local vinculado.
- `Hours`: Horas registradas.
- `SpentOn`: Fecha en que se realizó el trabajo (UTC).

## Endpoints

### GET /time-entries
**Acceso**: Administrator, HumanResources, ProjectManager

**Descripción**: Lista todos los time entries de un empleado en un rango de fechas. No paginado.

**Query Parameters**:
- `employeeId` (Guid, opcional): Filtrar por empleado.
- `from` (DateTime, requerido): Fecha inicio (UTC).
- `to` (DateTime, requerido): Fecha fin (UTC).

**Response** (200 OK): Lista de time entries.

---

### GET /time-entries/paged
**Acceso**: Administrator, HumanResources, ProjectManager

**Descripción**: Lista time entries con paginación.

**Query Parameters**:
- `page` (int): Página.
- `pageSize` (int): Cantidad por página.
- `employeeId` (Guid, opcional): Filtrar por empleado.
- `from` (DateTime, opcional): Fecha inicio.
- `to` (DateTime, opcional): Fecha fin.

**Response** (200 OK):
```json
{
  "items": [
    {
      "id": "guid",
      "redmineTimeEntryId": 1234,
      "redmineProjectId": 5,
      "employeeId": "guid",
      "hours": 8.0,
      "spentOn": "2026-03-01T00:00:00Z"
    }
  ],
  "totalCount": 120,
  "page": 1,
  "pageSize": 10
}
```

**Response** (204 No Content): Si no hay resultados.

## Notas
- Los time entries solo se crean vía sincronización con Redmine (no hay endpoint de creación manual).
- No se actualizan ni eliminan después de creados (insert-only).
- Las fechas deben ser UTC.
