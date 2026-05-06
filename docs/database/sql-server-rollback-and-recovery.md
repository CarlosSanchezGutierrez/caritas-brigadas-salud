# SQL Server Rollback and Recovery

## Objetivo

Definir los mínimos operativos para recuperación de SQL Server antes de producción.

## Conceptos

- RPO: pérdida máxima aceptable de datos.
- RTO: tiempo máximo aceptable para restaurar servicio.
- Restore probado: restauración validada en ambiente no productivo.

## Requisitos antes de producción

- Definir RPO.
- Definir RTO.
- Configurar backups automáticos.
- Definir retención.
- Probar restore.
- Documentar responsable.
- Documentar procedimiento de emergencia.

## Antes de cada migración productiva

- Confirmar último backup exitoso.
- Confirmar posibilidad de restore.
- Revisar si la migración es reversible.
- Preparar plan de rollback.
- Registrar versión de aplicación e imagen.

## Rollback de aplicación

- Mantener imagen anterior disponible.
- Revertir a tag anterior si el esquema sigue compatible.
- No borrar tags productivos anteriores hasta cerrar ventana de observación.

## Rollback de base de datos

Rollback de base de datos puede requerir restore completo si hubo cambios destructivos.

No asumir que todas las migraciones EF son reversibles en producción.

## Restore probado

Antes de datos reales, TI debe demostrar al menos un restore probado en ambiente controlado.
