# OpenAPI baseline

La API expone documentación OpenAPI durante desarrollo.

## URLs locales esperadas

- Swagger UI: `/swagger`
- OpenAPI JSON: `/openapi/v1/openapi.json`

## Reglas

- OpenAPI debe reflejar contratos reales.
- No documentar endpoints clínicos autónomos de IA.
- No exponer información sensible en ejemplos.
- Todo endpoint futuro debe declarar respuestas esperadas.
- Los clientes Web, iOS y Android deben alinearse con este contrato.
