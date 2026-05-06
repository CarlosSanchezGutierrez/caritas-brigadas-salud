# E2E Testing Baseline

## Objetivo

Validar que la interfaz institucional funcione de extremo a extremo a nivel de navegación, estados principales y contratos básicos de UI.

## Herramienta

- Playwright.

## Estrategia actual

- E2E con mocked API para CI/local estable.
- No depende de datos reales.
- No usa PHI.
- No usa PII real.
- Valida dashboard, reportes, auditoría y sistema.

## mocked API

Los tests interceptan llamadas a la API para validar UI sin requerir backend real en cada ejecución.

Esto reduce flakiness y evita depender de SQL Server en pruebas rápidas de frontend.

## Comandos

```powershell
cd apps/web-next
npm run test:e2e:list
npm run test:e2e
```

## CI

El CI lista las pruebas E2E para validar que el suite sea detectable. La ejecución completa puede habilitarse después en workflow dedicado.

## No PHI

Nunca usar datos reales de pacientes en E2E.
