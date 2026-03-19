# 03 - Integración con Redmine

## Descripción General

El sistema se integra con Redmine para obtener información de usuarios, proyectos, hitos (versiones) y time entries. La sincronización puede ejecutarse de forma automática (background service) o manual a través de endpoints dedicados.

## Configuración

Las credenciales y URL de Redmine se configuran en appsettings.json o variables de entorno:
```json
"Redmine": {
  "BaseUrl": "http://redmine:3000",
  "ApiKey": "a58b3860c4df76d18caba13d2dbe7b2aa4c13f0f"
}
```

**Recomendaciones**:
- Usar un usuario de Redmine con permisos de solo lectura.
- Proteger la ApiKey (no hardcodear, usar secretos).

## Background Service

El servicio `RedmineSyncBackgroundService` ejecuta sincronizaciones periódicas según la configuración:
```json
"RedmineSyncSchedule": {
  "Enabled": true,
  "IntervalHours": 24,
  "TimeEntryLookBackDays": 30,
  "InitialDelaySeconds": 30
}
```

- Al iniciar la aplicación, espera `InitialDelaySeconds` y luego ejecuta el primer ciclo.
- Posteriormente, ejecuta cada `IntervalHours` horas.
- Sincroniza: usuarios, proyectos, hitos y time entries de los últimos `TimeEntryLookBackDays` días.

## Endpoints de Sincronización Manual

### POST /redmine/sync-users

**Acceso**: Administrator, HumanResources

**Descripción**: Sincroniza usuarios desde Redmine. Crea nuevos empleados y actualiza nombres/emails de los existentes. Los usuarios que ya no existen en Redmine (y no son administradores) se desactivan.

**Response (200 OK)**:
```json
{
  "message": "User synchronization completed",
  "createdEmployees": 5
}
```

### POST /redmine/sync-projects

**Acceso**: Administrator, HumanResources

**Descripción**: Sincroniza proyectos desde Redmine. Crea nuevos proyectos y actualiza nombre/estado de los existentes. Los proyectos que ya no existen en Redmine (y no están completados) se marcan como cancelados.

**Response (200 OK)**:
```json
{
  "message": "Project synchronization completed",
  "createdProjects": 2
}
```

### POST /redmine/sync-milestones

**Acceso**: Administrator, HumanResources

**Descripción**: Sincroniza hitos (versiones) de todos los proyectos. Crea nuevos hitos y actualiza el estado de los existentes según la información de Redmine.

**Response (200 OK)**:
```json
{
  "message": "Milestones synchronization completed",
  "createdMilestones": 8
}
```

### POST /redmine/sync-time-entries

**Acceso**: Administrator

**Descripción**: Sincroniza time entries de los últimos 30 días (o el período configurado) para todos los empleados activos con RedmineUserId > 0. Crea nuevas entradas y actualiza las existentes.

**Response (200 OK)**:
```json
{
  "message": "Time entry synchronization completed",
  "createdTimeEntries": 150
}
```

## Mapeo de Estados de Hitos

| Estado en Redmine | Estado en sistema |
|------------------|------------------|
| open | Pending |
| closed | Completed |
| locked | Cancelled |

## Notas

- La sincronización es idempotente: no crea duplicados.
- Los time entries se sincronizan con las horas registradas; las aprobaciones se gestionan por separado (ver módulo de Time Entries).
- Si Redmine no está disponible, la sincronización falla con reintentos automáticos (política de reintentos configurada). El sistema sigue funcionando con los últimos datos conocidos.
