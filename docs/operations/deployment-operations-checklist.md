# Deployment Operations Checklist

## Antes de desplegar

- Verify verde en GitHub Actions.
- Imagen Docker construida correctamente.
- Vulnerability scan revisado.
- Secrets configurados fuera del repo.
- Connection string SQL Server productiva validada.
- CORS con dominio HTTPS real.
- AllowedHosts explícito.
- Auth real JwtBearer/OIDC configurado.
- Health probes configurados.
- Observabilidad configurada.
- Backups SQL Server activos.

## Durante despliegue

- Usar tag único de imagen.
- Aplicar migraciones con proceso separado.
- Verificar /health/live.
- Verificar /health/ready.
- Verificar login/auth.
- Verificar endpoint crítico con usuario autorizado.
- Verificar logs sin datos sensibles.

## Rollback

- Mantener imagen anterior disponible.
- Mantener release notes por versión.
- No borrar tag desplegado.
- Documentar migraciones no reversibles.
- Ejecutar rollback de app antes de rollback de base si aplica.

## Después del despliegue

- Revisar métricas.
- Revisar errores 5xx.
- Revisar 401/403 anómalos.
- Revisar latencia.
- Revisar SQL Server.
- Confirmar backups.
