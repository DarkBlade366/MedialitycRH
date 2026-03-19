# 06 - Reglas de Nómina

## Descripción General

Las reglas definen cómo se calculan los componentes de la nómina. Todas las reglas pueden estar activas o inactivas. Solo una regla activa de cada tipo (salario base por rol, horas extra, etc.) puede existir al mismo tiempo.

## 6.1. Salario Base (BaseSalaryRule)

### POST /base-salary-rules

**Acceso**: Administrator

**Descripción**: Crea una nueva regla de salario base.

**Request Body**:
```json
{
  "role": "Employee",
  "amount": 3000.00
}
```

**Response (200 OK)**:
```json
{
  "id": "guid",
  "role": "Employee",
  "amount": 3000.00,
  "isActive": true
}
```

### GET /base-salary-rules

**Acceso**: Administrator

**Descripción**: Lista reglas de salario base con paginación y filtros.

**Query Parameters**:
- `page` (int, requerido)
- `pageSize` (int, requerido)
- `isActive` (bool, opcional)
- `role` (EmployeeRole, opcional)

**Response (200 OK)**: Lista paginada.

### GET /base-salary-rules/{id}

**Acceso**: Administrator

**Descripción**: Obtiene una regla por ID.

### PUT /base-salary-rules/{id}/status

**Acceso**: Administrator

**Descripción**: Activa o desactiva una regla.

**Request Body**:
```json
{
  "isActive": true
}
```

**Response (204 No Content)**

## 6.2. Horas Extra (OvertimeRule)

### POST /overtime-rules

**Acceso**: Administrator

**Request Body**:
```json
{
  "standardHoursPerPeriod": 160,
  "overtimeMultiplier": 1.5,
  "hourlyRate": 25.0
}
```

**Response**:
```json
{
  "id": "guid",
  "standardHoursPerPeriod": 160,
  "overtimeMultiplier": 1.5,
  "hourlyRate": 25.0,
  "isActive": true
}
```

## 6.3. Deducciones (DeductionRule)

### POST /deduction-rules

**Acceso**: Administrator

**Request Body**:
```json
{
  "description": "ISR",
  "percentage": 0.08,
  "type": "BasicSalary"  // o "TotalEarnings"
}
```

**Response**:
```json
{
  "id": "guid",
  "description": "ISR",
  "percentage": 0.08,
  "type": "BasicSalary",
  "isActive": true
}
```

## 6.4. Aguinaldo (AguinaldoRule)

### POST /aguinaldo-rules

**Acceso**: Administrator

**Request Body**:
```json
{
  "monthlyAccrualPercentage": 0.08333,
  "payMonth": 12
}
```

**Response**:
```json
{
  "id": "guid",
  "monthlyAccrualPercentage": 0.08333,
  "payMonth": 12,
  "isActive": true
}
```

## 6.5. Vacaciones (VacationRule)

### POST /vacation-rules

**Acceso**: Administrator

**Request Body**:
```json
{
  "accrualRatePerMonth": 1.25
}
```

**Response**:
```json
{
  "id": "guid",
  "accrualRatePerMonth": 1.25,
  "isActive": true
}
```

## 6.6. Productividad (ProductivityRule)

### POST /productivity-rules

**Acceso**: Administrator

**Request Body**:
```json
{
  "minimumTarget": 80,
  "fullBonusTarget": 100,
  "bonusValue": 500,
  "bonusType": "FixedAmount",  // o "Percentage"
  "maxBonusCap": 1000
}
```

**Response**:
```json
{
  "id": "guid",
  "minimumTarget": 80,
  "fullBonusTarget": 100,
  "bonusValue": 500,
  "bonusType": "FixedAmount",
  "maxBonusCap": 1000,
  "isActive": true
}
```

## 6.7. Hitos (MilestoneRule)

### POST /milestone-rules

**Acceso**: Administrator

**Request Body**:
```json
{
  "redmineProjectId": 123,
  "milestoneName": "Fase 1",
  "bonusAmount": 2000
}
```

**Response**:
```json
{
  "id": "guid",
  "redmineProjectId": 123,
  "milestoneName": "Fase 1",
  "bonusAmount": 2000,
  "isActive": true
}
```

## 6.8. Proyectos (ProjectRule)

### POST /project-payment-rules

**Acceso**: Administrator, HumanResources

**Request Body**:
```json
{
  "redmineProjectId": 123,
  "bonusAmount": 5000
}
```

**Response**:
```json
{
  "id": "guid",
  "redmineProjectId": 123,
  "bonusAmount": 5000,
  "isActive": true
}
```

## Notas Generales

- Todas las reglas tienen endpoints similares para listar, obtener por ID y cambiar estado.
- Solo puede haber una regla activa de cada tipo (salario base por rol, horas extra, etc.). Al activar una nueva, se debe desactivar la anterior.
- Las reglas de hitos y proyectos requieren que el proyecto/milestone exista en la base de datos (sincronizado desde Redmine).
