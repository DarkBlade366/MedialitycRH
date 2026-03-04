# 08 - Vacaciones

## Descripción General
El sistema gestiona vacaciones de empleados con tres funcionalidades principales:
1. **Acumulación automática**: Un background service suma días de vacaciones mensualmente según la regla activa.
2. **Uso de vacaciones**: Los empleados pueden consumir días acumulados.
3. **Pago en nómina**: Al generar una nómina, los días usados se pagan según el salario base diario.

## Modelo de Balance (EmployeeVacationBalance)
Cada empleado tiene un balance de vacaciones con:
- `AccruedDays`: Días acumulados totales.
- `UsedDays`: Días consumidos (pendientes de pago).
- `AvailableDays`: `AccruedDays - UsedDays` (calculado).
- `LastAccrualDate`: Fecha de la última acumulación (para evitar doble acumulación en el mismo mes).

## Regla de Vacaciones (VacationRule)
La regla activa define el `AccrualRatePerMonth` — la cantidad de días que se acumulan mensualmente (ej: `3.5`). Solo puede haber una regla activa a la vez. Ver doc 06 para gestión de reglas.

## Acumulación Automática
El `VacationAccrualBackgroundService` ejecuta la acumulación:
1. Al **iniciar la aplicación**: Verifica si el mes actual ya fue acumulado. Si no, acumula inmediatamente.
2. En el **día programado** de cada mes (configurable en `appsettings.json`).

**Lógica de acumulación** (`VacationAccrualService`):
- Obtiene todos los empleados activos.
- Busca la regla de vacaciones activa (`IsActive = true`).
- Para cada empleado: si `HasAccruedThisMonth()` es `false`, suma `AccrualRatePerMonth` días.
- `HasAccruedThisMonth()` compara año y mes de `LastAccrualDate` con la fecha actual UTC.

**Configuración** (`appsettings.json`):
```json
{
  "VacationAccrualSchedule": {
    "Enabled": true,
    "RunDayOfMonth": 4,
    "RunHourUtc": 9,
    "RunMinuteUtc": 18
  }
}
```

## Endpoints

### GET /employees/{EmployeeId}/vacation-balance
**Acceso**: Administrator, HumanResources

**Descripción**: Consulta el balance de vacaciones de un empleado.

**Response** (200 OK):
```json
{
  "employeeId": "guid",
  "accruedDays": 15.0,
  "usedDays": 5.0,
  "availableDays": 10.0
}
```

---

### POST /employees/{EmployeeId}/use-vacation
**Acceso**: Administrator, HumanResources, ProjectManager, Employee

**Descripción**: Registra uso de días de vacaciones. Resta del balance disponible.

**Request Body**:
```json
{
  "employeeId": "guid",
  "days": 3
}
```

**Response** (200 OK)

**Validaciones**:
- `days` debe ser mayor a 0.
- No se pueden usar más días de los disponibles (`AvailableDays`).

## Pago de Vacaciones en Nómina
Al crear una nómina (`POST /payrolls`), si el empleado tiene `UsedDays > 0`, el `VacationCalculator` calcula:
```
dailyRate = BaseSalary / 30
amount = dailyRate × VacationDaysUsed
```
Se genera un componente `VacationPay` y se registra un `VacationPayment`.

## Notas
- La acumulación se ejecuta automáticamente al arrancar la app si no se ha hecho en el mes actual.
- Si la app está caída durante el día programado, la acumulación se recupera al reiniciar.
- El balance se persiste en la tabla `employee_vacation_balances`.
- Los días acumulados usan precisión `numeric(5,2)` (máximo 999.99 días).
