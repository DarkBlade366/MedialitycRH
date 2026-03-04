# 09 - Servicios en Background

## Descripción General
La aplicación cuenta con tres background services que se ejecutan automáticamente:
1. **RedmineSyncBackgroundService** — Sincronización periódica con Redmine.
2. **VacationAccrualBackgroundService** — Acumulación mensual de vacaciones.
3. **MonthlyPayrollBackgroundService** — Generación automática de nóminas mensuales.

Todos se registran como `IHostedService` en `Program.cs` y se configuran desde `appsettings.json`.

## 1. RedmineSyncBackgroundService
**Propósito**: Sincroniza proyectos, usuarios, milestones y time entries desde Redmine.

**Comportamiento**:
- Espera 30 segundos al iniciar la app antes del primer ciclo.
- Ejecuta 4 syncs secuencialmente: Proyectos → Usuarios → Milestones → Time Entries.
- Si un sync falla, loguea el error y continúa con el siguiente.
- Después de cada ciclo, espera `IntervalHours` horas.
- Si `Enabled = false`, revisa cada 5 minutos si se habilitó.

**Configuración**:
```json
{
  "RedmineSyncSchedule": {
    "Enabled": true,
    "IntervalHours": 24,
    "TimeEntryLookBackDays": 30
  }
}
```

**Parámetros**:
- `Enabled`: Activa/desactiva la sincronización automática.
- `IntervalHours`: Horas entre cada ciclo completo (mínimo 1).
- `TimeEntryLookBackDays`: Días hacia atrás para buscar time entries en cada ciclo.

---

## 2. VacationAccrualBackgroundService
**Propósito**: Acumula días de vacaciones mensualmente para todos los empleados activos.

**Comportamiento**:
- Al **iniciar la app**: ejecuta inmediatamente una verificación de acumulación. Si el mes actual no se ha acumulado, lo hace al instante (previene pérdida de acumulación por reinicios).
- Luego entra en el loop programado: espera hasta el día/hora configurado de cada mes.
- Usa `HasAccruedThisMonth()` del balance del empleado para evitar doble acumulación.
- Si `Enabled = false`, revisa cada 5 minutos si se habilitó.

**Configuración**:
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

**Parámetros**:
- `Enabled`: Activa/desactiva la acumulación automática.
- `RunDayOfMonth`: Día del mes para ejecutar (1-31, se ajusta si el mes tiene menos días).
- `RunHourUtc`: Hora UTC de ejecución (0-23).
- `RunMinuteUtc`: Minuto UTC de ejecución (0-59).

---

## 3. MonthlyPayrollBackgroundService
**Propósito**: Genera nóminas automáticamente para todos los empleados activos.

**Comportamiento**:
- Espera hasta el día/hora configurado de cada mes.
- Calcula el período: desde el primer día del mes anterior hasta el primer día del mes actual.
- Para cada empleado activo: verifica si ya existe una nómina para el período. Si no, la crea usando `CreatePayrollHandler`.
- Reporta resultado: total de empleados, nóminas creadas, omitidas y fallidas.
- Si `Enabled = false`, revisa cada 5 minutos si se habilitó.

**Configuración**:
```json
{
  "PayrollSchedule": {
    "Enabled": true,
    "RunDayOfMonth": 4,
    "RunHourUtc": 9,
    "RunMinuteUtc": 0
  }
}
```

**Parámetros**:
- `Enabled`: Activa/desactiva la generación automática.
- `RunDayOfMonth`: Día del mes para ejecutar (1-31).
- `RunHourUtc`: Hora UTC de ejecución (0-23).
- `RunMinuteUtc`: Minuto UTC de ejecución (0-59).

## Orden de Ejecución Recomendado
Cuando los tres servicios están configurados para el mismo día:
1. **Redmine Sync** (primero) — Asegura datos actualizados de time entries y milestones.
2. **Vacation Accrual** — Acumula vacaciones del mes.
3. **Monthly Payroll** (último) — Genera nóminas con datos actualizados.

Ejemplo de configuración temporal:
- Redmine Sync: continuo cada 24h
- Vacation Accrual: día 4, 09:18 UTC
- Monthly Payroll: día 4, 09:00 UTC

## Logs
Todos los servicios generan logs informativos y de error para monitoreo:
```
[INF] Startup vacation accrual check executed at 2026-03-04T09:22:00Z.
[INF] Vacation accrual scheduler waiting until 2026-04-04T09:18:00Z.
[INF] Monthly payroll executed for period 2026-02-01 - 2026-03-01. TotalEmployees=10, Created=10, Skipped=0, Failed=0
[INF] Redmine sync cycle completed successfully at 2026-03-04T09:00:12Z.
```

## Notas
- Si la app se reinicia después de la hora programada, el accrual de vacaciones se ejecuta al arrancar (los otros dos servicios esperan al siguiente mes).
- Todos los servicios manejan errores internamente sin detener la aplicación.
- Las fechas y horas se manejan en UTC.
