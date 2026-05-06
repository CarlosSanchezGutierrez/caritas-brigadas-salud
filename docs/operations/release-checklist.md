# Release Checklist

## pre-release

- Verify verde.
- SBOM generado.
- Trivy sin CRITICAL/HIGH bloqueantes.
- Release notes preparadas.
- Migraciones revisadas.
- Script SQL idempotente generado si aplica.
- Backup confirmado si aplica.
- Variables de entorno revisadas.
- Secrets configurados fuera del repo.
- Observabilidad lista.
- Rollback definido.

## production approval

- Aprobación técnica.
- Aprobación institucional.
- Ventana de cambio definida si aplica.
- Responsable de despliegue asignado.
- Responsable de monitoreo asignado.

## release

- Usar tag único de imagen.
- Aplicar migraciones con proceso separado.
- Desplegar aplicación.
- Verificar /health/live.
- Verificar /health/ready.
- Verificar autenticación.
- Verificar logs.

## post-release

- Revisar métricas.
- Revisar errores 5xx.
- Revisar 401/403.
- Revisar SQL Server.
- Confirmar backups.
- Documentar incidencias.
