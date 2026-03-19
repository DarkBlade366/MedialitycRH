# 08 - Analíticas y Reportes

## Descripción General

Endpoints para obtener información agregada y reportes de negocio.

### GET /analytics/project-costs

**Acceso**: Administrator, HumanResources, ProjectManager

**Descripción**: Calcula el costo estimado por proyecto basado en horas aprobadas y salarios base.

**Query Parameters**:
- `periodStart` (DateTime, requerido)
- `periodEnd` (DateTime, requerido)
- `projectId` (int, opcional): Filtrar por proyecto específico.

**Response (200 OK)**:
```json
[
  {
    "redmineProjectId": 101,
    "projectName": "Proyecto Alpha",
    "totalHours": 450.5,
    "estimatedCost": 12500.75,
    "contributions": [
      {
        "employeeId": "guid",
        "employeeName": "Juan Pérez",
        "hours": 200.0,
        "cost": 6000.00
      }
    ]
  }
]
```

### GET /analytics/hours-comparison/{employeeId}

**Acceso**: Administrator, HumanResources, ProjectManager

**Descripción**: Compara horas registradas vs esperadas para un empleado en un período.

**Path Parameter**: employeeId (Guid)

**Query Parameters**:
- `periodStart` (DateTime, requerido)
- `periodEnd` (DateTime, requerido)

**Response (200 OK)**:
```json
{
  "employeeId": "guid",
  "employeeName": "Juan Pérez",
  "periodStart": "2026-03-01T00:00:00Z",
  "periodEnd": "2026-03-31T23:59:59Z",
  "registeredHours": 170.0,
  "expectedHours": 160.0,
  "difference": 10.0,
  "percentage": 106.25
}
```

### GET /analytics/productivity-trend/{employeeId}

**Acceso**: Administrator, HumanResources, ProjectManager

**Descripción**: Obtiene la tendencia mensual de productividad ponderada por actividad.

**Path Parameter**: employeeId (Guid)

**Query Parameters**:
- `from` (DateTime, requerido): Mes de inicio.
- `to` (DateTime, requerido): Mes de fin.

**Response (200 OK)**:
```json
[
  {
    "year": 2026,
    "month": 3,
    "metric": 125.4
  },
  {
    "year": 2026,
    "month": 4,
    "metric": 130.2
  }
]
```
