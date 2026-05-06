# Secrets Management Baseline

Este documento define el estándar mínimo para manejar secretos del proyecto Cáritas Brigadas de Salud.

## Regla principal

Ningún secreto productivo debe almacenarse en el repositorio.

Esto incluye:

- Connection strings reales de SQL Server.
- Client secrets de OIDC.
- JWT signing keys.
- API keys.
- Credenciales de correo, storage, observability o terceros.

## Fuentes permitidas

- GitHub Actions Secrets para CI/CD.
- Azure Key Vault si Cáritas/Tec usa Azure.
- AWS Secrets Manager si se usa AWS.
- Variables de entorno administradas por la plataforma de deployment.
- Secret store institucional aprobado por TI.

## Variables backend mínimas

- ConnectionStrings__SqlServer
- Authentication__Authority
- Authentication__Audience
- Authentication__ValidIssuer
- Authentication__ValidAudiences__0

## SQL Server

La connection string productiva debe venir de un secret manager o variable de entorno segura.

Debe cumplir:

- Encrypt=True, Encrypt=Mandatory o Encrypt=Strict.
- TrustServerCertificate=False.
- No LocalDB.
- No localhost.
- No loopback.
- Usuario de mínimo privilegio.

## Archivos locales

appsettings.Local.json es solo para desarrollo y no debe commitearse.

Los archivos .example pueden existir, pero no deben contener secretos reales.

## Rotación

Todo secreto productivo debe poder rotarse sin cambiar código fuente.

## Incidente

Si un secreto real entra al repositorio:

1. Revocar o rotar inmediatamente.
2. Remover del historial si aplica.
3. Documentar incidente.
4. Revisar permisos afectados.
