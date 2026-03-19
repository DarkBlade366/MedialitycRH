# 07 - Nóminas (Payrolls)

## Descripción General

Las nóminas se generan por empleado y período. El cálculo utiliza las reglas activas y los time entries aprobados. Las nóminas pasan por los estados: Draft (borrador, mientras se agregan componentes), Calculated (calculado, listo para aprobación), Approved (aprobado) y Paid (pagado).

## Endpoints

### POST /payrolls

**Acceso**: Administrator, HumanResources

**Descripción**: Crea una nueva nómina para un empleado en un período.

**Request Body**:
```json
{
  "employeeId": "guid",
  "periodStart": "2026-03-01T00:00:00Z",
  "periodEnd": "2026-03-31T23:59:59Z"
}
```

**Response (200 OK)**:
```json
{
  "id": "guid",
  "employeeId": "guid",
  "periodStart": "2026-03-01T00:00:00Z",
  "periodEnd": "2026-03-31T23:59:59Z",
  "grossAmount": 3500.00,
  "totalDeductions": 280.00,
  "netAmount": 3220.00,
  "status": "Calculated",
  "components": [
    {
      "type": "BaseSalary",
      "category": "Earning",
      "description": "Base Salary",
      "amount": 3000.00
    },
    {
      "type": "Overtime",
      "category": "Earning",
      "description": "Overtime 10 hours",
      "amount": 375.00
    },
    {
      "type": "LegalDeduction",
      "category": "Deduction",
      "description": "ISR",
      "amount": 240.00
    }
  ]
}
```

### GET /payrolls

**Acceso**: Administrator

**Descripción**: Lista nóminas con paginación y filtros.

**Query Parameters**:
- `page` (int, requerido)
- `pageSize` (int, requerido)
- `status` (string, opcional): Draft, Calculated, Approved, Paid
- `employeeId` (Guid, opcional)
- `periodStart` (DateTime, opcional)
- `periodEnd` (DateTime, opcional)

**Response (200 OK)**: Lista paginada de nóminas.

### GET /payrolls/{id}

**Acceso**: Administrator

**Descripción**: Obtiene una nómina por ID.

**Path Parameter**: id (Guid)

**Response (200 OK)**: Mismo formato que la creación.

### POST /payrolls/approved/{id}

**Acceso**: Administrator, HumanResources

**Descripción**: Aprueba una nómina (cambia estado a Approved). Solo puede aprobarse si está en estado Calculated.

**Path Parameter**: id (Guid)

**Response (200 OK)**: La nómina actualizada.

### POST /payrolls/paid/{id}

**Acceso**: Administrator, HumanResources

**Descripción**: Marca una nómina como pagada (estado Paid). Solo puede pagarse si está en estado Approved.

**Path Parameter**: id (Guid)

**Response (200 OK)**: La nómina actualizada.

### GET /payrolls/{id}/pdf

**Acceso**: Administrator, HumanResources

**Descripción**: Genera y descarga un recibo de nómina en PDF.

**Query Parameter**:
- `lang` (string, opcional): es o en. Por defecto es.

**Response**: Archivo PDF.

### GET /payrolls/{id}/excel

**Acceso**: Administrator, HumanResources

**Descripción**: Genera y descarga un recibo de nómina en Excel.

**Query Parameter**:
- `lang` (string, opcional): es o en.

**Response**: Archivo Excel (.xlsx).

## Notas

- No se puede generar una nómina si ya existe otra en el mismo período para el mismo empleado.
- No se puede generar si hay time entries pendientes de aprobación en el período.
- El cálculo incluye:
  - Salario base (según rol)
  - Horas extra (si aplica)
  - Deducciones
  - Acumulación de aguinaldo (si hay regla activa)
  - Pago de aguinaldo (si el mes coincide con el mes de pago)
  - Uso de vacaciones (si se registraron)
  - Bonos por hitos completados (si hay participaciones activas)
  - Bonos por proyectos completados (si hay regla)
  - Bono de productividad (si hay regla activa)
