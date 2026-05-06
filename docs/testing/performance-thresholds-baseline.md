# Performance Thresholds Baseline

## Objetivo

Definir umbrales iniciales para evitar degradación operativa.

## API smoke inicial

- error rate menor a 1%.
- p95 menor a 750 ms.
- sin errores 5xx recurrentes.
- sin timeouts.

## Métricas futuras

- p50.
- p95.
- p99.
- throughput.
- CPU.
- memoria.
- conexiones SQL Server.
- latencia SQL Server.

## Criterio de producción

Los thresholds productivos deben definirse con TI después de conocer volumen real de brigadas, usuarios concurrentes y tamaño de datos.
