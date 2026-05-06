# Threat Model

## Activos principales

- Datos personales de pacientes.
- Consentimientos y avisos de privacidad.
- Información clínica capturada en brigadas.
- Usuarios, roles, permisos y auditoría.
- Reportes institucionales.
- Base de datos SQL Server.

## Amenazas cubiertas inicialmente

- SQL injection.
- CORS mal configurado.
- Acceso sin autenticación.
- Acceso sin permisos.
- IDOR por organización.
- Dependencias vulnerables.
- Stack traces o errores inseguros.
- Fugas por logs sensibles.
- Uso accidental de autenticación de desarrollo en producción.
- Uso accidental de localhost o HTTP en producción.

## Controles ya implementados

- Authorization por permisos.
- Validación de acceso por organización.
- Audit logs.
- Security headers.
- Rate limiting.
- Max request body size.
- HTTPS redirection.
- HSTS fuera de Development.
- Build con warnings como errores.
- npm audit y NuGet vulnerable scan.
- Production configuration validation.

## Riesgos pendientes

- Autenticación real todavía no implementada.
- Gestión de secretos pendiente.
- Observabilidad productiva pendiente.
- Despliegue reproducible pendiente.
- Pruebas de carga pendientes.
- Branch protection pendiente.
- Política formal de retención de datos pendiente.

## Regla operativa

No usar datos reales de pacientes hasta cerrar autenticación real, gestión de secretos, infraestructura productiva, observabilidad y aprobación institucional.
