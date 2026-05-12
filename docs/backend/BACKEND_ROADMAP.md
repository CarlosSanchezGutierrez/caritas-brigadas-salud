# Backend Roadmap — P2 to P12

Este roadmap define el trabajo backend posterior a P0/P1.

Objetivo: continuar backend antes de frontend, con PRs pequenos, verificables y sin deuda tecnica.

---

## P2 — Data integrity baseline

Prioridad: Alta

Objetivo: endurecer consistencia de datos.

Tareas:

- Revisar constraints SQL.
- Revisar indices.
- Revisar claves foraneas.
- Revisar unicidad por organizacion.
- Revisar cascades.
- Revisar soft delete si aplica.
- Revisar timestamps.
- Revisar CreatedAt/UpdatedAt.
- Revisar SubmittedAt.
- Revisar campos nullable.
- Revisar reglas de negocio en entidades.
- Agregar tests de integridad.
- Agregar contratos para migraciones.

Resultado esperado:

- Base de datos mas segura.
- Menos riesgo de datos huerfanos.
- Menos riesgo de duplicados.
- Mejor rendimiento inicial.

---

## P3 — API contract hardening

Prioridad: Alta

Objetivo: hacer que la API sea mas estable y profesional.

Tareas:

- Revisar rutas.
- Normalizar responses.
- Normalizar errores.
- Revisar paginacion.
- Revisar filtros.
- Revisar sorting.
- Revisar status codes.
- Revisar OpenAPI/Swagger.
- Revisar versionado `/api/v1`.
- Validar DTOs.
- Revisar nombres de requests/responses.
- Revisar consistencia Created/Ok/NotFound/Conflict/Forbidden.
- Agregar tests de contratos HTTP.

Resultado esperado:

- API mas limpia.
- Menos cambios rompientes para frontend/iOS/Android.

---

## P4 — Audit and traceability advanced baseline

Prioridad: Alta

Objetivo: mejorar trazabilidad institucional.

Tareas:

- Auditar operaciones write.
- Auditar exports.
- Auditar seeds.
- Auditar asignacion de roles.
- Auditar cambios de usuarios.
- Auditar creacion de pacientes.
- Auditar consentimientos.
- Auditar sync batches.
- Definir payloads no sensibles.
- Evitar PII en logs.
- Revisar correlation id.
- Revisar actor id.
- Revisar organization id.
- Agregar pruebas de auditoria.

Resultado esperado:

- Mejor cumplimiento.
- Mejor investigacion de incidentes.
- Mejor confianza institucional.

---

## P5 — Observability baseline

Prioridad: Media-alta

Objetivo: preparar operacion real.

Tareas:

- Logs estructurados.
- Metrics.
- Traces.
- Health checks mas detallados.
- Readiness real con DB.
- Liveness simple.
- Alertas.
- Request telemetry.
- Error telemetry.
- Dashboard tecnico.
- Documentar eventos.
- Revisar niveles de log.

Resultado esperado:

- Backend observable.
- Mejor debugging.
- Mejor operacion en produccion.

---

## P6 — Security hardening advanced

Prioridad: Alta

Objetivo: endurecer abuso y amenazas.

Tareas:

- Threat model.
- Abuse cases.
- Rate limit por endpoint.
- Rate limit por actor.
- Rate limit para writes.
- Payload limits por endpoint.
- Validacion CORS.
- Revisar security headers.
- Revisar antiforgery si aplica.
- Revisar JWT/OIDC claims.
- Revisar clock skew.
- Revisar authorization handlers.
- Revisar tenant boundary tests.
- Revisar error leakage.
- Revisar DDoS posture.

Resultado esperado:

- Menos superficie de ataque.
- Mejor preparacion para produccion real.

---

## P7 — Testing advanced baseline

Prioridad: Media-alta

Objetivo: ampliar confianza tecnica.

Tareas:

- Integration tests por modulo.
- API E2E tests.
- Tests de permisos por rol.
- Tests de tenant boundary.
- Tests de seeds idempotentes.
- Tests de migraciones.
- Tests de CSV exports.
- Tests de validation errors.
- Smoke test local.
- Smoke test Docker.
- Load tests basicos.
- Regression tests para bugs detectados por Codex.

Resultado esperado:

- Menos regresiones.
- Mejor capacidad de entregar a nuevas generaciones.

---

## P8 — Infrastructure and deployment baseline

Prioridad: Alta, dependiente de entorno real

Objetivo: preparar despliegue serio.

Tareas:

- Definir environments.
- Definir secrets.
- Azure Key Vault o equivalente.
- SQL Server deployment model.
- Backups.
- Restore drill.
- Docker compose.
- IaC inicial.
- Reverse proxy.
- TLS.
- CORS por dominio real.
- CI/CD release pipeline.
- Rollback plan.
- Runbooks.

Resultado esperado:

- Deploy reproducible.
- Menor dependencia de configuraciones manuales.

---

## P9 — Offline and sync robustness

Prioridad: Alta para producto movil real

Objetivo: soportar brigadas con conectividad irregular.

Tareas:

- Revisar sync batches.
- Idempotency keys.
- Conflict resolution.
- Device id.
- SubmittedAt.
- Client generated ids.
- Retry strategy.
- Partial failures.
- Sync audit.
- Deduplication.
- Sync status.
- Offline data contracts.
- Mobile API assumptions.

Resultado esperado:

- Base lista para iOS/Android en campo.

---

## P10 — Reporting and analytics baseline

Prioridad: Media

Objetivo: preparar direccion y analitica.

Tareas:

- Reportes operativos.
- CSV/XLSX exports.
- Agregados por organizacion.
- Agregados por brigada.
- Metricas por servicio.
- Metricas por comunidad.
- Metricas por periodo.
- Data contracts para BI.
- Preparacion para Power BI.
- Preparacion para warehouse futuro.
- PII minimization.

Resultado esperado:

- Reportes utiles para direccion.
- Base de datos preparada para analitica.

---

## P11 — LLM API Gateway

Prioridad: Futuro estrategico

Objetivo: dejar arquitectura preparada para integrar modelos LLM sin comprometer seguridad.

Tareas:

- API Gateway interno.
- Registro de prompts.
- Redaction de PII.
- Rate limits.
- Model provider abstraction.
- Audit de inferencias.
- Human-in-the-loop.
- Policy de uso.
- No decisiones medicas autonomas.
- Validacion legal/etica.

Resultado esperado:

- IA integrable de forma controlada.

---

## P12 — Cryptographic audit / blockchain research track

Prioridad: Futuro academico/experimental

Objetivo: abrir linea para alumnos interesados en criptografia.

Tareas:

- Hash chains para eventos.
- Merkle roots de auditoria.
- Firmas digitales.
- Evidencia verificable.
- No blockchain innecesaria en MVP.
- Proof of concept aislado.
- Documentacion academica.
- Evaluacion costo/beneficio.

Resultado esperado:

- Linea de investigacion sin contaminar el core productivo.

---

## Orden recomendado inmediato

1. Cerrar docs P0/P1.
2. P2 Data integrity.
3. P3 API contract hardening.
4. P4 Audit advanced.
5. P6 Security hardening advanced.
6. P7 Testing advanced.
7. P8 Infra/deploy.
8. P9 Offline/sync.
9. P10 Analytics.
10. P11/P12 como tracks futuros.

---

## Regla de ejecucion

Cada fase debe dividirse en PRs pequenos.

Ningun PR debe mezclar:

- DB changes
- Auth logic
- Controller contract changes
- Docs
- Frontend
- Infra
- Encoding cleanup

Si se detecta un bug, se corrige en hotfix pequeno antes de continuar.