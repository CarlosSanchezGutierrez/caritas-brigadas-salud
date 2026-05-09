# P0 Backend Pagination Baseline

## Estado

La fase **P0 Backend Pagination Baseline** queda cerrada como checkpoint técnico del backend.

Este checkpoint documenta la migración de los listados críticos del backend hacia un contrato paginado estándar, evitando respuestas no acotadas y preparando el backend para consumo estable desde los tres clientes previstos:

- Web
- iOS nativo en Swift
- Android

## Objetivo técnico

Estandarizar los endpoints críticos de lectura para que devuelvan respuestas paginadas mediante:

```csharp
ApiResponse<PaginatedResponse<TDto>>
```

en lugar de colecciones completas como:

```csharp
ApiResponse<IReadOnlyCollection<TDto>>
```

Con esto se reduce riesgo de:

- respuestas demasiado grandes;
- degradación de rendimiento;
- consumo excesivo de memoria;
- contratos inconsistentes entre clientes;
- comportamiento distinto entre Web, iOS y Android;
- deuda técnica temprana en endpoints de alto volumen.

## Contrato estándar

Los listados P0 usan el contrato:

```csharp
public sealed record PaginatedResponse<T>
{
    public IReadOnlyCollection<T> Items { get; init; }
    public int PageNumber { get; init; }
    public int PageSize { get; init; }
    public int TotalCount { get; init; }
    public int TotalPages { get; }
}
```

Los controladores reciben paginación desde query string:

```csharp
[FromQuery] PaginationRequest pagination
```

Ejemplo de consumo HTTP:

```http
GET /api/v1/organizations/{organizationId}/patients?pageNumber=1&pageSize=50
```

Respuesta esperada:

```json
{
  "success": true,
  "data": {
    "items": [],
    "pageNumber": 1,
    "pageSize": 50,
    "totalCount": 0,
    "totalPages": 0
  },
  "message": null,
  "traceId": "...",
  "timestampUtc": "..."
}
```

## Reglas de paginación

La paginación se normaliza desde `PaginationRequest`.

Reglas principales:

- `pageNumber` menor a 1 usa el valor por defecto.
- `pageSize` menor a 1 usa el valor por defecto.
- `pageSize` mayor al máximo se limita al máximo permitido.
- `pageNumber` excesivamente grande se limita para evitar overflow en `Skip`.
- `Skip` se calcula de forma segura.
- El repositorio debe ejecutar `CountAsync` para obtener `TotalCount`.
- El repositorio debe aplicar ordenamiento determinístico antes de `Skip` y `Take`.

## Endpoints P0 migrados

| Área | Endpoint | DTO | Estado |
|---|---|---|---|
| Audit logs | `GET /api/v1/organizations/{organizationId}/audit-logs` | `AuditLogSummaryDto` | Paginado |
| Patients | `GET /api/v1/organizations/{organizationId}/patients` | `PatientSummaryDto` | Paginado |
| Users | `GET /api/v1/organizations/{organizationId}/users` | `UserSummaryDto` | Paginado |
| Sync batches | `GET /api/v1/organizations/{organizationId}/sync-batches` | `SyncBatchSummaryDto` | Paginado |
| Consent documents | `GET /api/v1/organizations/{organizationId}/consent-documents` | `ConsentDocumentSummaryDto` | Paginado |
| Form responses | `GET /api/v1/organizations/{organizationId}/form-responses` | `FormResponseSummaryDto` | Paginado |
| Patient visits | `GET /api/v1/organizations/{organizationId}/patient-visits` | `PatientVisitSummaryDto` | Paginado |

## Reglas aplicadas en repositorios

Cada repositorio P0 debe cumplir:

```csharp
var totalCount = await query.CountAsync(cancellationToken);

var items = await query
    .OrderBy(...)
    .ThenBy(...)
    .Skip(pagination.Skip)
    .Take(pageSize)
    .ToArrayAsync(cancellationToken);
```

Criterios mínimos:

- `AsNoTracking()` en lecturas.
- Filtro por `OrganizationId`.
- Exclusión de registros eliminados lógicamente cuando aplique.
- Ordenamiento antes de `Skip` y `Take`.
- `Id` como desempate determinístico cuando aplique.
- Sin `.Take(250)` como límite fijo sustituto de paginación.
- Sin `ToListAsync` antes de paginar.

## Ordenamiento crítico

### Form responses

Debe conservar orden clínico/cronológico:

1. `SubmittedAt`
2. `CapturedAt`
3. `CreatedAt`
4. `Id` como desempate

### Patient visits

Debe conservar orden operativo por llegada:

1. `ArrivalTime`
2. `Id` como desempate

### Audit logs

Debe conservar orden de evento/auditoría, priorizando temporalidad y trazabilidad.

## Impacto en clientes

### Web

El cliente Web debe leer listados paginados desde:

```ts
response.data?.items ?? []
```

No desde:

```ts
response.data ?? []
```

### iOS Swift

Los clientes iOS deben modelar respuestas como:

```swift
struct ApiResponse<T: Decodable>: Decodable {
    let success: Bool
    let data: T?
    let message: String?
    let traceId: String
    let timestampUtc: String
}

struct PaginatedResponse<T: Decodable>: Decodable {
    let items: [T]
    let pageNumber: Int
    let pageSize: Int
    let totalCount: Int
    let totalPages: Int
}
```

### Android

Los clientes Android deben modelar el mismo contrato:

```kotlin
data class ApiResponse<T>(
    val success: Boolean,
    val data: T?,
    val message: String?,
    val traceId: String,
    val timestampUtc: String
)

data class PaginatedResponse<T>(
    val items: List<T>,
    val pageNumber: Int,
    val pageSize: Int,
    val totalCount: Int,
    val totalPages: Int
)
```

## Validación aplicada

La fase P0 incluye validaciones de:

- `dotnet build` con warnings como errores.
- `dotnet test` con warnings como errores.
- pruebas de contrato de paginación por endpoint.
- verificación de que los controladores usan `PaginatedResponse<TDto>`.
- verificación de que los controladores reciben `PaginationRequest`.
- verificación de que los repositorios usan `CountAsync`, `Skip` y `Take`.
- validación de frontend cuando aplica.
- actualización de mocks E2E para respetar respuestas paginadas.
- release de baseline a `main`.

## Pruebas de contrato agregadas

Se agregaron pruebas ligeras por fuente para asegurar que los endpoints P0 no regresen a contratos no paginados.

Áreas cubiertas:

- Audit logs
- Patients
- Users
- Sync batches
- Consent documents
- Form responses
- Patient visits

## Fuera de alcance de este checkpoint

Este checkpoint no resuelve todavía:

- índices SQL definitivos;
- análisis de query plans;
- optimización fina de N+1;
- estrategia formal de caching;
- rate limiting;
- observabilidad completa;
- OpenAPI endurecido;
- documentación maestra completa;
- paginación de endpoints P1;
- generación formal de clientes Swift/Kotlin/TypeScript;
- pruebas E2E completas de todos los flujos clínicos;
- pruebas de carga;
- colas/event-driven architecture;
- dead letter queues;
- arquitectura de notificaciones en tiempo real.

## P1 pendiente

Después de P0, los listados P1 recomendados son:

1. `service-encounters`
2. `brigades`
3. `form-templates`
4. `communities`
5. `mobile-units`
6. `roles`
7. `services`
8. `brigade-services`

## Criterio para iniciar P1

Antes de iniciar P1 debe cumplirse:

- `develop` limpio.
- `main` actualizado con el baseline P0.
- sin PRs abiertos.
- build y tests pasando.
- documentación checkpoint P0 mergeada.
- ramas temporales limpiadas.

## Decisión arquitectónica

Se mantiene un enfoque de **monolito modular** con contratos HTTP consistentes para que Web, iOS Swift y Android consuman el mismo backend sin crear lógica divergente por cliente.

La prioridad de esta fase fue reducir deuda técnica temprana en endpoints de lectura críticos antes de avanzar a optimización, seguridad avanzada, OpenAPI formal y clientes móviles.