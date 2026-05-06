# Secret Scanning and Push Protection Baseline

## Objetivo

Evitar que secretos entren al repositorio y detectar secretos que ya estén visibles.

## Controles GitHub

- Secret scanning.
- Push protection.
- Dependabot alerts.
- Dependabot security updates.
- Dependency graph.

## GitHub Settings

Configurar manualmente en GitHub:

1. Settings.
2. Advanced Security.
3. Enable Secret Protection.
4. Enable Push protection.
5. Enable Dependency graph.
6. Enable Dependabot alerts.
7. Enable Dependabot security updates.

## Regla operativa

no secrets en el repositorio.

No commitear:

- Connection strings reales.
- Tokens.
- Client secrets.
- API keys.
- Passwords.
- Private keys.
- Certificados privados.

## Si GitHub bloquea un push

1. No hacer bypass por comodidad.
2. Remover el secreto del commit.
3. Rotar el secreto si ya fue expuesto.
4. Revisar historial si llegó a publicarse.
5. Documentar incidente si era un secreto real.

## Bypass

Un bypass solo debe aceptarse si es falso positivo comprobado.

## Producción

Los secretos productivos deben vivir en Azure Key Vault, GitHub Secrets, secret manager institucional o variables seguras de la plataforma.
