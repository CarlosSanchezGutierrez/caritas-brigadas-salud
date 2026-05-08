# System Overview

## Propósito

Este documento explica la arquitectura general sin obligar a leer todo el código.

## Vista general

El sistema se organiza como una plataforma web con backend API, frontend web y base de datos SQL Server.

```text
Usuario / Colaborador / TI
        |
        v
Next.js Web App
        | HTTPS
        v
ASP.NET Core API
        |
        v
SQL Server
```

## Backend

Ubicación:

- services/api-dotnet

Estructura esperada:

- Domain: reglas centrales y entidades de dominio.
- Application: casos de uso, contratos internos y lógica de aplicación.
- Infrastructure: persistencia, proveedores externos y detalles técnicos.
- Contracts: DTOs y contratos expuestos.
- Api: controladores, middleware, autenticación, autorización y configuración HTTP.

## Frontend

Ubicación:

- apps/web-next

Responsabilidades:

- Presentar la interfaz institucional.
- Consumir la API.
- Preparar flujos operativos para brigadas.
- Mantener separación entre UI, servicios de API y configuración.

## Base de datos

Proveedor objetivo:

- Microsoft SQL Server.

Principios:

- Migraciones controladas.
- Scripts idempotentes cuando aplique.
- Usuarios mínimos.
- Backups y recuperación antes de producción.
- No usar datos reales en desarrollo local.

## Seguridad

Controles actuales:

- Autenticación de desarrollo separada de autenticación real.
- Boundary para OIDC/JWT.
- Validaciones de configuración productiva.
- Headers de seguridad.
- Rate limiting baseline.
- Auditoría HTTP y acciones clínicas/operativas.
- CORS controlado.
- Dependency Review.
- SBOM.
- Trivy.
- Secret scanning y push protection en GitHub.

## CI/CD

Workflows principales:

- Verify.
- Repository Security.

Gates principales:

- Backend security and quality gate.
- Frontend security and quality gate.
- Deployment baseline metadata gate.
- Database deployment baseline metadata gate.
- Repository governance metadata gate.
- Supply chain baseline metadata gate.
- Testing baseline metadata gate.
- Docker image build gate.
- Repository security metadata gate.
- Dependency Review.

## Qué significa production-ready baseline

Significa que el repositorio tiene controles técnicos, documentación y estructura para acercarse a producción de forma seria.

No significa que ya esté desplegado en producción ni que ya tenga aprobación final de TI, legal o negocio.
