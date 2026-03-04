# 03 - Sincronización con Redmine

## Descripción General
El sistema se integra con Redmine para extraer datos de usuarios, proyectos, milestones y time entries. La sincronización opera de dos formas:

1. **Manual**: Endpoints POST que un administrador puede invocar en cualquier momento.
2. **Automática (CRON)**: Un `BackgroundService` que ejecuta los 4 syncs en orden, con intervalo configurable (por defecto cada 24 horas).

## Configuración Redmine (appsettings.json)
```json
{
  "Redmine": {
    "BaseUrl": "http://localhost:3000",
    "ApiKey": "TU_API_KEY_DE_REDMINE"
  },
  "RedmineSyncSchedule": {
    "Enabled": true,
    "IntervalHours": 24,
    "TimeEntryLookBackDays": 30
  }
}
```

### Parámetros del CRON
- **Enabled**: `true` para activar la sincronización automática, `false` para desactivarla.
- **IntervalHours**: Cada cuántas horas se ejecuta un ciclo completo. Mínimo: 1.
- **TimeEntryLookBackDays**: Cuántos días hacia atrás buscar time entries en cada ciclo. Default: 30.

## Orden de Sincronización
El CRON ejecuta los syncs en este orden (cada uno es independiente, si uno falla los demás continúan):

1. **Proyectos** → Crea/actualiza/elimina proyectos locales según Redmine.
2. **Usuarios (Empleados)** → Crea empleados nuevos, actualiza nombres, desactiva los que ya no están en Redmine.
3. **Milestones** → Crea milestones nuevos y actualiza estados (open→Pending, closed→Completed, locked→Cancelled).
4. **Time Entries** → Crea time entries nuevos (no actualiza ni elimina existentes).

## Endpoints Manuales

### POST /redmine/sync-projects
**Acceso**: Administrator, HumanResources

**Descripción**: Sincroniza proyectos desde Redmine. Crea proyectos nuevos, actualiza nombres de existentes, y elimina los que ya no existen en Redmine.

**Response** (200 OK):
```json
{
  "message": "Project synchronization completed",
  "createdProjects": 3
}
```

---

### POST /redmine/sync-users
**Acceso**: Administrator

**Descripción**: Sincroniza usuarios de Redmine como empleados. Los nuevos se crean con rol `Employee` y password temporal `Temp1234`. Los que ya no existen en Redmine se desactivan (excepto Administradores).

**Response** (200 OK):
```json
{
  "message": "User synchronization completed",
  "createdEmployees": 5
}
```

---

### POST /redmine/sync-milestones
**Acceso**: Administrator, HumanResources

**Descripción**: Sincroniza milestones (versions) de todos los proyectos en Redmine. Mapeo de estados:
- `open` → Pending
- `closed` → Completed
- `locked` → Cancelled

**Response** (200 OK):
```json
{
  "message": "Milestones synchronization completed",
  "createdMilestones": 8
}
```

---

### POST /redmine/sync-time-entries
**Acceso**: Administrator

**Descripción**: Sincroniza time entries de los últimos 30 días para todos los empleados activos con RedmineUserId. Solo crea entries nuevos (no actualiza ni elimina).

**Response** (200 OK):
```json
{
  "message": "Time entry synchronization completed",
  "createdTimeEntries": 42
}
```

## Comportamiento del CRON (RedmineSyncBackgroundService)
- Espera 30 segundos al iniciar la app antes del primer ciclo.
- Ejecuta los 4 syncs secuencialmente en cada ciclo.
- Si un sync falla, loguea el error y continúa con el siguiente.
- Después de completar un ciclo, espera `IntervalHours` horas.
- Si `Enabled = false`, revisa cada 5 minutos si se habilitó.
- Todos los eventos se registran en los logs de la aplicación.

## Logs
El servicio genera logs informativos y de error:
```
[INF] Redmine sync cycle starting at 2026-03-04T09:00:00Z.
[INF] Redmine projects synced. Created: 2.
[INF] Redmine users synced. Created: 0.
[INF] Redmine milestones synced. Created: 4.
[INF] Redmine time entries synced (2026-02-02 to 2026-03-04). Created: 15.
[INF] Redmine sync cycle completed successfully at 2026-03-04T09:00:12Z.
[INF] Next Redmine sync in 24 hour(s).
```
