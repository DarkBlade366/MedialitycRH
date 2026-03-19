# 10 - Operaciones

## 10.1. Runbooks

### Diagnóstico de problemas comunes

#### API no responde o error 500

- Verificar contenedores: `docker ps`
- Revisar logs: `docker logs payments_api --tail 100`
- Verificar conectividad con PostgreSQL y Redis.
- Revisar Seq en http://servidor:5341.

#### Error de sincronización con Redmine

- Verificar que el servicio esté habilitado.
- Revisar credenciales en variables de entorno.
- Probar conectividad: `curl -I http://redmine:3000`
- Ejecutar sincronización manual vía endpoints.

#### Lentitud en consultas

- Verificar índices en PostgreSQL.
- Comprobar que Redis esté funcionando.

### Backup y Restauración

**Backup diario (ejemplo cron)**:
```bash
0 2 * * * docker exec payments_postgres pg_dump -U postgres paymentsdb > /backups/backup_$(date +\%Y\%m\%d).sql
```

**Restauración**:
```bash
docker stop payments_api
cat backup_20260319.sql | docker exec -i payments_postgres psql -U postgres paymentsdb
docker start payments_api
```

## 10.2. Plan de Recuperación ante Desastres (DRP)

- **RPO**: 24 horas (backups diarios).
- **RTO**: 4 horas.

**Procedimiento**:
1. Detener servicios: `docker-compose down`
2. Iniciar solo PostgreSQL: `docker-compose up -d postgres`
3. Restaurar backup.
4. Iniciar resto de servicios: `docker-compose up -d`
5. Verificar integridad.

## 10.3. Handover a Operaciones

**Checklist de puesta en marcha**:
- Configurar variables de entorno (JWT, Redmine, conexiones).
- Ejecutar `docker-compose up -d`.
- Verificar migraciones automáticas.
- Probar login con admin por defecto.
- Probar sincronización manual.
- Configurar backups automáticos.
- Monitorear logs en Seq.

**Contactos**:
- Desarrollo: [Xavier Ramirez Fernandez \ Medialityc]
- Repositorio: [https://github.com/DarkBlade366/MedialitycRH.git]
