# 11 - Configuración General

## Variables de Entorno

El sistema se configura mediante variables de entorno (producción) o appsettings.json (desarrollo).

```bash
# Base de datos
ConnectionStrings__DbMedialitycHR=Host=postgres;Port=5432;Database=paymentsdb;Username=postgres;Password=xxxx
ConnectionStrings__Redis=redis:6379

# JWT
Jwt__Key=tu_clave_secreta_muy_larga
Jwt__Issuer=Payments.Api
Jwt__Audience=Payments.Users
Jwt__AccessTokenMinutes=60

# Redmine
Redmine__BaseUrl=http://redmine:3000
Redmine__ApiKey=tu_api_key

# Schedulers
PayrollSchedule__Enabled=true
PayrollSchedule__RunDayOfMonth=4
PayrollSchedule__RunHourUtc=9
PayrollSchedule__RunMinuteUtc=0

VacationAccrualSchedule__Enabled=true
VacationAccrualSchedule__RunDayOfMonth=4
VacationAccrualSchedule__RunHourUtc=9
VacationAccrualSchedule__RunMinuteUtc=29

RedmineSyncSchedule__Enabled=true
RedmineSyncSchedule__IntervalHours=24
RedmineSyncSchedule__TimeEntryLookBackDays=30
RedmineSyncSchedule__InitialDelaySeconds=30
```

## Docker Compose

El archivo docker-compose.yml incluye:

- API
- PostgreSQL
- Redis
- Seq (logs)

Para levantar el entorno completo:
```bash
docker-compose up -d
```

## Migraciones

Las migraciones de base de datos se aplican automáticamente al iniciar la API (en Program.cs se ejecuta `dbContext.Database.MigrateAsync()`).

## Seed

El AdminSeeder crea un administrador por defecto si no existe ninguno.
