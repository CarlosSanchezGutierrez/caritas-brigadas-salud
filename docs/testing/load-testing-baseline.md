# Load Testing Baseline

## Objetivo

Definir pruebas de carga controladas para validar comportamiento inicial de API sin castigar el CI normal.

## Herramienta

- k6.

## Estrategia

- Pruebas manuales/controladas.
- No ejecutar carga pesada en cada push.
- Usar ambientes staging o local controlado.
- No usar datos reales.

## Script

- tests/load/api-smoke-load-test.js

## Comando local

```powershell
k6 run tests/load/api-smoke-load-test.js
```

## Variables

- BASE_URL.
- DEV_ORGANIZATION_ID.
- DEV_USER_ID.
- K6_VUS.
- K6_DURATION.

## thresholds

- http_req_failed rate menor a 1%.
- p95 menor a 750 ms para smoke inicial.

## manual

Las pruebas de carga deben ejecutarse manualmente o en workflow dedicado con aprobación.
