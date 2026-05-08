# Cáritas Brigadas de Salud

Plataforma institucional para apoyar la operación digital de brigadas de salud, con enfoque en trazabilidad, seguridad, auditoría, reportes y evolución mantenible por estudiantes y equipos técnicos.

Este repositorio está diseñado como una base seria para colaboración entre Cáritas, Tec de Monterrey y futuros contribuidores técnicos. No es solo una app: es un sistema preparado para crecer con gobierno de repositorio, controles de seguridad, pruebas, documentación operativa y una arquitectura modular.

## Estado del repositorio

- Backend ASP.NET Core con arquitectura por capas.
- Frontend Next.js como app shell institucional.
- SQL Server como base de datos objetivo.
- Docker baseline para empaquetado de API.
- GitHub Actions con gates de calidad, seguridad, Docker, supply chain, testing y gobernanza.
- SBOM generado en CI.
- Dependency Review vía REST API para bloquear vulnerabilidades high y critical sin annotations ruidosas.
- Branch protection y rulesets para proteger develop y main.
- Documentación de producción, seguridad, testing, deployment y operación.

## Lectura recomendada

Empieza aquí:

- docs/START_HERE.md
- docs/architecture/system-overview.md
- docs/architecture/folder-map.md
- docs/contributing/local-development.md
- docs/contributing/contribution-paths.md
- docs/governance/maintainer-playbook.md
- docs/operations/ti-handoff.md
- docs/security/security-map.md

## Regla principal

Nadie debe meter cambios a develop o main sin Pull Request y sin que pasen los checks requeridos.

## Qué NO es este repositorio todavía

- No es todavía un despliegue productivo final.
- No debe almacenar datos reales de pacientes en el repositorio.
- No debe contener secretos, connection strings reales ni llaves privadas.
- No reemplaza aprobación institucional, revisión legal ni validación final de TI.

## Qué sí representa

Una base técnica seria para convertir el proyecto en un sistema institucional mantenible, auditable y escalable.
