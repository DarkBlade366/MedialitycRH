# 07 - Nómina (Payroll)

## Descripción General
El módulo de nómina permite crear, aprobar, pagar y consultar nóminas de empleados. Al crear una nómina, el motor de cálculo (`PayrollEngine`) aplica todas las reglas activas y genera los componentes de pago correspondientes.

## Flujo de la Nómina (Workflow)
```
Creación → Pendiente (Pending) → Aprobada (Approved) → Pagada (Paid)
```

1. **Crear** (`POST /payrolls`): Se genera la nómina calculando todos los componentes. Estado: `Pending`.
2. **Aprobar** (`POST /payrolls/approved/{id}`): Un administrador/RRHH revisa y aprueba. Estado: `Approved`.
3. **Pagar** (`POST /payrolls/paid/{id}`): Se marca como pagada. Estado: `Paid`.

## Componentes Calculados
Al crear una nómina, el engine calcula y registra:
- **Salario Base**: Según la regla activa para el rol del empleado.
- **Horas Extra**: Basado en time entries de Redmine que excedan el umbral.
- **Productividad**: Bonificación según horas registradas vs umbral.
- **Milestones**: Pagos por milestones completados donde el empleado participó.
- **Aguinaldo**: Proporción anual según regla activa.
- **Vacaciones**: Pago de vacaciones acumuladas.
- **Deducciones**: Aplicación de deducciones legales y configuradas.

## Endpoints

### POST /payrolls
**Acceso**: Administrator, HumanResources

**Descripción**: Crea una nómina para un empleado en un período específico. El motor aplica todas las reglas activas.

**Request Body**:
```json
{
  "employeeId": "guid-del-empleado",
  "periodStart": "2026-02-01T00:00:00Z",
  "periodEnd": "2026-03-01T00:00:00Z"
}
```

**Response** (200 OK): Objeto completo de la nómina con componentes.

---

### POST /payrolls/approved/{id}
**Acceso**: Administrator, HumanResources

**Descripción**: Aprueba una nómina pendiente. Cambia estado de Pending a Approved.

**Path Parameter**: `id` (Guid)

**Response** (200 OK): Nómina actualizada.

---

### POST /payrolls/paid/{id}
**Acceso**: Administrator, HumanResources

**Descripción**: Marca una nómina aprobada como pagada. Cambia estado de Approved a Paid.

**Path Parameter**: `id` (Guid)

**Response** (200 OK): Nómina actualizada.

---

### GET /payrolls
**Acceso**: Administrator

**Descripción**: Lista nóminas con paginación y filtros.

**Query Parameters**:
- `page`, `pageSize` (requeridos)

**Response** (200 OK): Lista paginada de nóminas.

---

### GET /payrolls/{id}
**Acceso**: Administrator

**Descripción**: Obtiene una nómina por su ID con todos sus componentes.

**Response** (200 OK / 404 Not Found)

---

### GET /payrolls/{id}/pdf
**Acceso**: Administrator, HumanResources

**Descripción**: Genera y descarga el recibo de pago en PDF.

**Response**: Archivo PDF (`application/pdf`).

---

### GET /payrolls/{id}/excel
**Acceso**: Administrator, HumanResources

**Descripción**: Genera y descarga el desglose de nómina en Excel.

**Response**: Archivo Excel (`application/vnd.openxmlformats-officedocument.spreadsheetml.sheet`).

## Generación Automática (Background Service)
Además de la creación manual, existe un `MonthlyPayrollBackgroundService` que genera nóminas automáticamente para todos los empleados activos el día configurado de cada mes (ver doc 09).

## Estados de la Nómina (PayrollStatus)
- `Pending` — Recién creada, pendiente de revisión (esta dividida en `Draft` y `Calculate`, pero nada mas que se crea se calcula, es mas que nada para saber si hubo algo error).
- `Approved` — Aprobada por administrador/RRHH.
- `Paid` — Pagada al empleado.
