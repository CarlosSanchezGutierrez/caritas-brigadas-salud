# SQL Server Migration Deployment Baseline

Este documento define cómo manejar migraciones SQL Server para Cáritas Brigadas de Salud.

## Regla central

No ejecutar migraciones automáticamente al arrancar la API.

La API runtime no debe modificar el esquema de base de datos al iniciar.

## Estrategia

1. Generar script SQL idempotente desde EF Core.
2. Revisar el script antes de ejecutarlo.
3. Ejecutarlo con usuario de migraciones.
4. Validar health/readiness después de aplicar.
5. Mantener evidencia de la versión aplicada.

## Generación local

```powershell
powershell -ExecutionPolicy Bypass -File scripts/db-generate-migration-script.ps1
```

Salida esperada:

- artifacts/db/caritas-brigadas-idempotent-migrations.sql

Ese artifact no debe contener secretos.

## Usuario runtime

El usuario runtime es el usuario que usa la API.

Debe tener mínimo privilegio:

- SELECT/INSERT/UPDATE según tablas necesarias.
- No debe ser sysadmin.
- No debe ser db_owner.
- No debe aplicar migraciones.

## Usuario de migraciones

El usuario de migraciones se usa solo en pipeline/proceso controlado.

Puede tener permisos para modificar esquema, pero no debe ser usado por la API en runtime.

## Ambientes

### Local

- LocalDB o SQL Server container.
- Migraciones pueden aplicarse por scripts locales.

### Staging

- Generar SQL idempotente.
- Revisar script.
- Ejecutar con usuario de migraciones.
- Validar endpoints y smoke tests.

### Production

- Backup previo obligatorio.
- Aprobación de TI.
- Ventana de cambio si aplica.
- Ejecutar con usuario de migraciones.
- Validar health, logs y métricas.
- Tener plan de rollback.

## Reglas

- No usar laptops personales para aplicar migraciones productivas.
- No usar usuario runtime para migraciones.
- No aplicar migraciones sin backup.
- No aplicar cambios destructivos sin plan de rollback.
- No modificar manualmente la base sin registrar cambio.
