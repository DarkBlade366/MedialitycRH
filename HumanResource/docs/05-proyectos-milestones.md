# 05 - Proyectos y Milestones

## Descripción General

Los proyectos y hitos se sincronizan desde Redmine. Los proyectos pueden tener reglas de pago asociadas (ProjectRule) y los hitos pueden tener participaciones de empleados que, al completarse, generan pagos por hito.

## Endpoints de Proyectos

### GET /projects

**Acceso**: Administrator, HumanResources, ProjectManager

**Descripción**: Lista proyectos con paginación.

**Query Parameters**:
- `page` (int, requerido)
- `pageSize` (int, requerido)

**Response (200 OK)**:
```json
{
  "items": [
    {
      "id": "guid",
      "redmineProjectId": 101,
      "name": "Proyecto Alpha"
    }
  ],
  "totalCount": 25,
  "page": 1,
  "pageSize": 10
}
```

### GET /projects/{redmineProjectId}

**Acceso**: Administrator, HumanResources, ProjectManager

**Descripción**: Obtiene un proyecto por su ID de Redmine.

**Path Parameter**: redmineProjectId (int)

**Response (200 OK)**: Mismo formato que el listado.

## Endpoints de Milestones

### GET /milestones

**Acceso**: Administrator, HumanResources, ProjectManager

**Descripción**: Lista hitos con filtros y paginación.

**Query Parameters**:
- `page` (int, requerido)
- `pageSize` (int, requerido)
- `redmineProjectId` (int, opcional)
- `status` (string, opcional): Pending, Completed, Cancelled
- `from` (DateTime, opcional): Fecha de completado desde (solo si status=Completed)
- `to` (DateTime, opcional): Fecha de completado hasta (solo si status=Completed)

**Response (200 OK)**:
```json
{
  "items": [
    {
      "id": "guid",
      "redmineProjectId": 101,
      "name": "Fase 1",
      "status": "Completed",
      "completedAt": "2026-03-10T14:30:00Z"
    }
  ],
  "totalCount": 8,
  "page": 1,
  "pageSize": 10
}
```

### GET /milestones/{id}

**Acceso**: Administrator, HumanResources, ProjectManager

**Descripción**: Obtiene un hito por su ID interno.

**Path Parameter**: id (Guid)

**Response (200 OK)**: Mismo formato que en el listado.

## Endpoints de Participaciones en Milestones

### POST /milestone-participations

**Acceso**: Administrator

**Descripción**: Crea una participación de un empleado en un hito.

**Request Body**:
```json
{
  "projectMilestoneId": "guid",
  "employeeId": "guid"
}
```

**Response (200 OK)**:
```json
{
  "id": "guid",
  "projectMilestoneId": "guid",
  "employeeId": "guid",
  "isPaid": false,
  "isActive": true
}
```

### GET /milestone-participations

**Acceso**: Administrator

**Descripción**: Lista participaciones con filtros.

**Query Parameters**:
- `page` (int, requerido)
- `pageSize` (int, requerido)
- `projectMilestoneId` (Guid, opcional)
- `isActive` (bool, opcional)

**Response (200 OK)**: Lista paginada de participaciones.

### GET /milestone-participations/{id}

**Acceso**: Administrator

**Descripción**: Obtiene una participación por su ID.

**Path Parameter**: id (Guid)

**Response (200 OK)**: Mismo formato que en la creación.

### PUT /milestone-participations/status

**Acceso**: Administrator

**Descripción**: Activa o desactiva una participación (solo si no ha sido pagada).

**Request Body**:
```json
{
  "id": "guid",
  "isActive": false
}
```

**Response (200 OK)**

## Notas

- Los proyectos se sincronizan automáticamente con Redmine. El estado se mapea: 1=Active, 5=Completed, 9=Cancelled.
- Los hitos se sincronizan y su estado se actualiza según la información de Redmine.
- Una participación marcada como pagada (`isPaid=true`) no puede modificarse (desactivarse) y no generará un nuevo pago en futuras nóminas.
