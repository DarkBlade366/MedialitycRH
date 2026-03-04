# 04 - Proyectos, Milestones y Participaciones

## Descripción General
Los proyectos y milestones se sincronizan desde Redmine (ver doc 03). Este módulo permite consultar la data sincronizada y gestionar las participaciones de empleados en milestones (para el cálculo de pagos por milestone).

## Proyectos

### GET /projects
**Acceso**: Administrator, HumanResources, ProjectManager

**Descripción**: Lista proyectos paginados (sincronizados desde Redmine).

**Query Parameters**:
- `page` (int): Página.
- `pageSize` (int): Cantidad por página.

**Response** (200 OK):
```json
{
  "items": [
    {
      "id": "guid",
      "redmineProjectId": 5,
      "name": "Proyecto Alpha"
    }
  ],
  "totalCount": 10,
  "page": 1,
  "pageSize": 10
}
```

---

### GET /projects/{RedmineProjectId}
**Acceso**: Administrator, HumanResources, ProjectManager

**Descripción**: Obtiene un proyecto por su RedmineProjectId.

**Path Parameter**: `RedmineProjectId` (int)

**Response** (200 OK / 404 Not Found)

## Milestones

### GET /milestones
**Acceso**: Administrator, HumanResources, ProjectManager

**Descripción**: Lista milestones paginados con filtros opcionales.

**Query Parameters**:
- `page`, `pageSize` (requeridos)
- `projectId` (int, opcional): Filtrar por proyecto.
- `status` (opcional): Filtrar por estado (Pending, Completed, Cancelled).
- `completedAtFrom`, `completedAtTo` (DateTime, opcional): Rango de fecha de completado.

**Response** (200 OK): Lista paginada de milestones.

---

### GET /milestones/{Id}
**Acceso**: Administrator, HumanResources, ProjectManager

**Descripción**: Obtiene un milestone por su ID (Guid).

**Response** (200 OK / 404 Not Found)

## Participaciones en Milestones

Las participaciones vinculan un empleado a un milestone específico. Se usan para calcular pagos por milestone en la nómina.

### POST /milestone-participations
**Acceso**: Administrator

**Descripción**: Crea una participación de un empleado en un milestone.

**Request Body**:
```json
{
  "projectMilestoneId": "guid-del-milestone",
  "employeeId": "guid-del-empleado"
}
```

**Response** (200 OK): Detalle de la participación creada.

---

### GET /milestone-participations
**Acceso**: Administrator

**Descripción**: Lista participaciones paginadas con filtros opcionales.

**Query Parameters**:
- `page`, `pageSize` (requeridos)
- Filtros opcionales por milestone y estado activo.

---

### GET /milestone-participations/{Id}
**Acceso**: Administrator

**Descripción**: Obtiene una participación por su ID.

**Response** (200 OK / 404 Not Found)

---

### PUT /milestone-participations/status
**Acceso**: Administrator

**Descripción**: Activa o desactiva una participación. No se puede desactivar si ya fue pagada.

**Request Body**:
```json
{
  "isActive": false
}
```

**Response** (200 OK)
