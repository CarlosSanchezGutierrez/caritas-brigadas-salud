# START HERE

Este documento es la puerta de entrada al repositorio.

Si eres nuevo en el proyecto, no empieces leyendo todo el código. Primero entiende las capas.

## En una frase

Cáritas Brigadas de Salud es una plataforma web y API para registrar, consultar, auditar y reportar información operativa de brigadas de salud, con una base preparada para SQL Server, seguridad institucional y contribución ordenada.

## Las cinco capas del proyecto

### 1. Producto

Lo que el usuario final ve y usa:

- Dashboard institucional.
- Reportes.
- Auditoría.
- Sistema.
- Futuras pantallas operativas para pacientes, brigadas, servicios, consentimiento y visitas.

### 2. Backend

La API central que expone endpoints, aplica reglas de negocio, valida permisos y conecta con infraestructura.

Ubicación principal:

- services/api-dotnet

### 3. Frontend

La aplicación web institucional construida con Next.js.

Ubicación principal:

- apps/web-next

### 4. Datos

La base objetivo es SQL Server. El proyecto ya considera migraciones, scripts idempotentes, backups, recuperación y despliegue controlado.

### 5. Gobierno, seguridad y operación

Controles que hacen que el repositorio sea serio:

- GitHub Actions.
- Rulesets.
- Required checks.
- Docker build.
- Trivy.
- SBOM.
- Dependency Review.
- Secret scanning.
- CodeQL cuando GitHub termine de indexar lenguajes.

## Qué leer según tu perfil

### Si eres alumno nuevo

Lee:

- docs/architecture/folder-map.md
- docs/contributing/local-development.md
- docs/contributing/contribution-paths.md

### Si eres parte de TI

Lee:

- docs/architecture/system-overview.md
- docs/security/security-map.md
- docs/operations/ti-handoff.md
- docs/operations/production-readiness.md

### Si eres maintainer

Lee:

- docs/governance/maintainer-playbook.md
- docs/governance/required-checks-baseline.md
- docs/governance/branch-protection-baseline.md

## Regla de oro

No modifiques main o develop directamente. Trabaja en una rama, abre Pull Request y espera checks verdes.

## Antes de tocar código

Confirma:

- Entiendes qué carpeta vas a tocar.
- Sabes qué check puede fallar.
- No estás agregando secretos.
- No estás usando datos reales.
- No estás rompiendo los contratos de API.
