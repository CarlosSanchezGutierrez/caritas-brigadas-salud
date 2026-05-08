# Maintainer Playbook

## Objetivo

Definir cómo mantener el repositorio sin perder control técnico.

## Responsabilidad del maintainer

Un maintainer no solo aprueba código. Protege arquitectura, seguridad, datos, claridad y continuidad del proyecto.

## Checklist antes de aprobar un PR

- El PR tiene alcance claro.
- No mezcla cambios no relacionados.
- No incluye secretos.
- No incluye datos reales.
- No degrada seguridad.
- No rompe contratos de API.
- No toca migraciones sin justificación.
- Todos los checks pasan.
- La documentación se actualiza si cambió comportamiento.

## Checks obligatorios

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

## Cuándo bloquear un PR

Bloquea si:

- Hay vulnerabilidades high o critical.
- Hay secretos.
- Hay datos reales.
- Hay bypass injustificado.
- Hay cambios masivos sin explicación.
- Hay deuda técnica evidente.
- Hay cambio de seguridad sin documentación.

## Cuándo pedir cambios

Pide cambios si:

- El código funciona pero no se entiende.
- El nombre de archivos o funciones no comunica intención.
- Falta prueba mínima.
- Falta documentación de una decisión importante.
- Hay duplicación innecesaria.

## Política de main

main representa estado estable/release.

No debe recibir cambios directos.

## Política de develop

develop representa integración activa.

Debe recibir cambios por PR con checks verdes.

## Política de seguridad

No aceptar cambios que debiliten seguridad solo para acelerar una entrega.

Si una regla bloquea por mala configuración, corregir la regla o documentar excepción temporal.
