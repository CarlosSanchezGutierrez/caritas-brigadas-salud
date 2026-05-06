# Observability Baseline

Este documento define el estándar mínimo de observabilidad para Cáritas Brigadas de Salud antes de un despliegue institucional.

## Objetivo

Detectar fallos, degradación, errores de seguridad y problemas operativos sin exponer datos sensibles de pacientes.

## Estado actual

- Logging de consola en ASP.NET Core.
- Middleware de correlación.
- Middleware de telemetría de requests.
- Security smoke local.
- Verify local.
- GitHub Actions Verify.
- Health endpoints.

## Request telemetry

El middleware registra:

- TraceId.
- Método HTTP.
- Ruta sanitizada.
- Status code.
- Duración en milisegundos.

No registra:

- Request body.
- Response body.
- Authorization headers.
- Cookies.
- Tokens.
- Connection strings.
- Datos clínicos.
- Datos personales de pacientes.

## Rutas sensibles

Las rutas de pacientes, visitas, encuentros clínicos, consentimientos, respuestas de formularios y sync batches se registran como /api/v1/[sensitive-resource].

## Métricas mínimas para producción

- Tasa de errores 4xx y 5xx.
- Latencia p50, p95 y p99.
- Throughput por endpoint.
- Health check status.
- Rate limiting 429.
- Errores de autenticación 401.
- Errores de autorización 403.
- Errores SQL Server.
- Tiempo de conexión a base de datos.
- Uso de CPU/memoria del host.

## Alertas mínimas

- API caída.
- Health check failed.
- Spike de 5xx.
- Spike de 401/403.
- Spike de 429.
- Latencia p95 alta.
- Error recurrente de SQL Server.
- Fallo de backups.
- Uso alto de almacenamiento.

## Herramientas recomendadas

La herramienta final debe definirla TI. Opciones compatibles:

- Azure Application Insights si se usa Azure.
- OpenTelemetry Collector.
- Grafana + Prometheus + Loki.
- Datadog.
- New Relic.
- Elastic Observability.

## Reglas de seguridad

- No loguear PHI/PII.
- No loguear body de requests.
- No loguear tokens.
- No loguear cookies.
- No loguear connection strings.
- No loguear payloads clínicos.
- Usar TraceId para correlacionar incidentes.

## Pendientes antes de producción

- Definir herramienta institucional.
- Configurar sink externo.
- Configurar dashboards.
- Configurar alertas.
- Configurar retención de logs.
- Documentar respuesta a incidentes.
- Probar trazabilidad completa de un request.
