# 06 - Reglas de Nómina

## Descripción General
Las reglas de nómina definen cómo se calculan los diferentes componentes del sueldo. Cada tipo de regla tiene sus propios endpoints de creación, consulta y cambio de estado. Solo las reglas activas (`IsActive = true`) se aplican al generar una nómina.

Todos los endpoints de reglas requieren rol **Administrator**.

## Tipos de Reglas

### 1. Base Salary (Salario Base)
Define el monto fijo mensual por rol.

**Endpoints**:
- `POST /base-salary-rules` — Crear regla
- `GET /base-salary-rules` — Listar paginado
- `GET /base-salary-rules/{id:guid}` — Obtener por ID
- `PUT /base-salary-rules/{id:guid}/status` — Cambiar estado

**Request de creación**:
```json
{
  "amount": 3000.00,
  "role": "Employee"
}
```

---

### 2. Overtime (Horas Extra)
Define el multiplicador para horas que excedan el umbral configurado. Se calculan desde las horas registradas en Redmine.

**Endpoints**:
- `POST /overtime-rules` — Crear regla
- `GET /overtime-rules` — Listar paginado
- `GET /overtime-rules/{id:guid}` — Obtener por ID
- `PUT /overtime-rules/{id:guid}/status` — Cambiar estado

---

### 3. Productivity (Productividad)
Define bonificaciones basadas en la productividad del empleado (horas registradas en Redmine vs umbrales configurados).

**Endpoints**:
- `POST /productivity-rules` — Crear regla
- `GET /productivity-rules` — Listar paginado
- `GET /productivity-rules/{id:guid}` — Obtener por ID
- `PUT /productivity-rules/{id:guid}/status` — Cambiar estado

---

### 4. Milestone
Define pagos por completar milestones de proyectos Redmine. Se vincula con las participaciones de empleados en milestones.

**Endpoints**:
- `POST /milestone-rules` — Crear regla
- `GET /milestone-rules` — Listar paginado
- `GET /milestone-rules/{id:guid}` — Obtener por ID
- `PUT /milestone-rules/{id:guid}/status` — Cambiar estado

---

### 5. Aguinaldo
Define las reglas de cálculo de aguinaldo (bono anual).

**Endpoints**:
- `POST /aguinaldo-rules` — Crear regla
- `GET /aguinaldo-rules` — Listar paginado
- `GET /aguinaldo-rules/{id:guid}` — Obtener por ID
- `PUT /aguinaldo-rules/{id:guid}/status` — Cambiar estado

---

### 6. Vacation (Vacaciones)
Define las reglas de acumulación y pago de vacaciones.

**Endpoints**:
- `POST /vacation-rules` — Crear regla
- `GET /vacation-rules` — Listar paginado
- `GET /vacation-rules/{id:guid}` — Obtener por ID
- `PUT /vacation-rules/{id:guid}/status` — Cambiar estado

---

### 7. Deduction (Deducciones)
Define deducciones aplicables (legales, seguridad social, etc.).

**Endpoints**:
- `POST /deduction-rules` — Crear regla
- `GET /deduction-rules` — Listar paginado
- `GET /deduction-rules/{id:guid}` — Obtener por ID
- `PUT /deduction-rules/{id:guid}/status` — Cambiar estado

## Patrón Común

Todos los tipos de regla siguen el mismo patrón:

### Crear regla: POST /{tipo}-rules
**Request**: Campos específicos de cada tipo + configuración.
**Response** (200 OK): Objeto de la regla creada con su ID.

### Listar paginado: GET /{tipo}-rules
**Query Parameters**: `page`, `pageSize`
**Response** (200 OK): Lista paginada de reglas.

### Obtener por ID: GET /{tipo}-rules/{id:guid}
**Response** (200 OK / 404 Not Found)

### Cambiar estado: PUT /{tipo}-rules/{id:guid}/status
**Request**:
```json
{
  "isActive": true
}
```
**Response** (204 No Content)

## Notas
- Solo puede haber una regla activa de cada tipo por rol/configuración (dependiendo del tipo).
- Al crear una nómina, el motor de cálculo (`PayrollEngine`) aplica todas las reglas activas correspondientes.
- Las reglas no se eliminan, solo se desactivan.
