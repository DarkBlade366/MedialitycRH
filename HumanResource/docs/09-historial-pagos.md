# 09 - Historial de Pagos

## Descripción General

Endpoints para consultar el historial de pagos realizados a través de las nóminas. Todos los endpoints tienen paginación y filtros similares.

### GET /aguinaldo-payments

**Acceso**: Administrator

**Filtros**:
- `payrollId` (Guid, opcional)
- `aguinaldoRuleId` (Guid, opcional)
- `from` (DateTime, opcional)
- `to` (DateTime, opcional)

**Response**: Lista paginada de pagos de aguinaldo.

### GET /deduction-payments

**Acceso**: Administrator

**Filtros similares**.

### GET /milestone-payments

**Acceso**: Administrator

**Filtros**:
- `payrollId`, `milestoneRuleId`, `from`, `to`

### GET /overtime-payments

**Acceso**: Administrator

### GET /productivity-payments

**Acceso**: Administrator

### GET /project-payments

**Acceso**: Administrator

### GET /vacation-payments

**Acceso**: Administrator
