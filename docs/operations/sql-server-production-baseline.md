# SQL Server Production Baseline

Cáritas actualmente usa SQL Server de Microsoft. Este proyecto debe alinearse a ese ecosistema para producción.

## Requisitos mínimos

- SQL Server administrado por TI o Azure SQL si la institución lo autoriza.
- TLS/encryption obligatorio.
- Backups automáticos.
- Recovery plan documentado.
- Usuario de aplicación con mínimo privilegio.
- Usuario separado para migraciones.
- Auditoría de accesos.
- Monitoreo de rendimiento y errores.

## Connection string productiva

Debe cumplir:

- Encrypt=True, Encrypt=Mandatory o Encrypt=Strict.
- TrustServerCertificate=False.
- Server remoto institucional.
- Sin LocalDB.
- Sin localhost.
- Sin 127.0.0.1.

## Usuarios recomendados

### Usuario de aplicación

- Lectura/escritura solo sobre tablas necesarias.
- Sin permisos de administración de servidor.
- Sin db_owner salvo justificación excepcional.

### Usuario de migraciones

- Usado solo por pipeline controlado.
- Permisos elevados solo para aplicar migraciones.
- No debe usarse por la API en runtime.

## Backups y recuperación

Antes de producción se debe definir:

- Frecuencia de backups.
- Retención.
- RPO.
- RTO.
- Procedimiento de restore probado.

## Migraciones

Las migraciones EF Core deben aplicarse mediante pipeline o procedimiento aprobado, no manualmente desde laptops personales.

## Datos sensibles

No cargar datos reales hasta cerrar autenticación real, secrets management, observabilidad, deployment controlado y aprobación institucional.
