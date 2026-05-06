# Container Image Release Strategy

## Regla principal

No usar latest en producción.

## Tags recomendados

- commit SHA.
- build ID.
- versión semántica cuando aplique.

Ejemplo:

- caritas-brigadas-api:sha-abc1234
- caritas-brigadas-api:build-20260506-001
- caritas-brigadas-api:v0.1.0

## Registry recomendado

Por alineación Microsoft:

- Azure Container Registry.

Alternativas portables:

- GitHub Container Registry.
- Registry institucional de TI.

## Rollback

Para rollback debe existir:

- Imagen anterior disponible.
- Tag anterior conocido.
- Release notes.
- Compatibilidad con esquema SQL Server actual.
- Procedimiento para revertir configuración.

## Promoción de ambientes

La misma imagen debe promoverse entre ambientes cuando sea posible:

- build once.
- scan once.
- promote to staging.
- approve.
- promote to production.

## No hacer

- No reconstruir una imagen diferente para producción si ya se validó otra en staging.
- No borrar tags recientemente desplegados.
- No desplegar imágenes sin SBOM.
- No desplegar imágenes con vulnerabilidades CRITICAL,HIGH sin aprobación formal.
