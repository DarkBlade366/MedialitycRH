# 10 - Historial de Pagos

## Descripción General
Al generar y pagar nóminas, el sistema registra los pagos individuales por cada componente. Estos endpoints permiten consultar el historial de pagos realizados, agrupados por tipo.

Todos los endpoints de historial de pagos:
- Requieren rol **Administrator**.
- Soportan paginación (`page`, `pageSize`).
- Retornan una respuesta paginada estándar.

## Endpoints

### GET /aguinaldo-payments
**Descripción**: Lista pagos de aguinaldo registrados.

**Query Parameters**:
- `page` (int, requerido)
- `pageSize` (int, requerido)

**Response** (200 OK): Lista paginada de pagos de aguinaldo con `aguinaldoRuleId`, `amount`, `paidAt`, `payrollId`.

---

### GET /deduction-payments
**Descripción**: Lista deducciones aplicadas en nóminas.

**Query Parameters**:
- `page` (int, requerido)
- `pageSize` (int, requerido)

**Response** (200 OK): Lista paginada de deducciones con `deductionRuleId`, `amount`, `appliedAt`, `payrollId`.

---

### GET /milestone-payments
**Descripción**: Lista pagos por milestones completados.

**Query Parameters**:
- `page` (int, requerido)
- `pageSize` (int, requerido)

**Response** (200 OK): Lista paginada de pagos de milestone con `milestoneRuleId`, `amount`, `paidAt`, `payrollId`.

---

### GET /overtime-payments
**Descripción**: Lista pagos por horas extra.

**Query Parameters**:
- `page` (int, requerido)
- `pageSize` (int, requerido)

**Response** (200 OK): Lista paginada de pagos de horas extra con `overtimeRuleId`, `amount`, `paidAt`, `payrollId`.

---

### GET /productivity-payments
**Descripción**: Lista pagos por bonificación de productividad.

**Query Parameters**:
- `page` (int, requerido)
- `pageSize` (int, requerido)

**Response** (200 OK): Lista paginada de pagos de productividad con `productivityRuleId`, `amount`, `paidAt`, `payrollId`.

---

### GET /vacation-payments
**Descripción**: Lista pagos por vacaciones usadas.

**Query Parameters**:
- `page` (int, requerido)
- `pageSize` (int, requerido)

**Response** (200 OK): Lista paginada de pagos de vacaciones con `vacationRuleId`, `amount`, `paidAt`, `payrollId`.

## Formato de Respuesta Común
Todos los endpoints retornan el mismo formato paginado:
```json
{
  "items": [
    {
      "id": "guid",
      "payrollId": "guid",
      "amount": 500.00,
      "paidAt": "2026-03-01T00:00:00Z"
    }
  ],
  "totalCount": 25,
  "page": 1,
  "pageSize": 10
}
```
Cada tipo de pago incluye adicionalmente el ID de la regla que lo generó (ej: `aguinaldoRuleId`, `deductionRuleId`, etc.).

## Notas
- Los pagos se crean automáticamente al generar una nómina (no hay endpoints de creación manual).
- Son registros de solo lectura (insert-only).
- Cada pago está vinculado a una nómina específica (`payrollId`).
