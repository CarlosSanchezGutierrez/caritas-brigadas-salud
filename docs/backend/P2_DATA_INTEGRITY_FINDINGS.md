# P2 Data Integrity Findings

Estado: P2 en progreso  
Base: develop despues de P2-01  
Objetivo: documentar hallazgos antes de modificar migraciones, foreign keys o delete behavior.

---

## 1. Contexto

P0 y P1 dejaron estabilizada la base de despliegue, seguridad, roles, permisos, autorizacion, seeds, UTF-8 y documentacion maestra.

P2 se enfoca en integridad de datos:

- Primary keys.
- Client-generated IDs.
- Required fields.
- Max lengths.
- Unique indexes.
- Lookup indexes.
- Foreign keys.
- Delete behavior.
- Tenant boundary.
- Soft delete consistency.
- SQL baseline alignment.

P2-01 ya agrego contratos ejecutables del modelo EF para:

- Primary keys `Id`.
- `ValueGeneratedNever` en entidades derivadas de `Entity`.
- `CreatedAt`.
- `IsDeleted`.
- Indices criticos unicos.
- Indices criticos de consulta.
- Longitudes y required flags de campos importantes.

---

## 2. Hallazgo principal

El modelo actual tiene buena base de propiedades, indices y convenciones de auditoria/soft delete, pero todavia debe endurecer la integridad relacional.

Hay muchos campos de relacion por identificador:

- `OrganizationId`
- `UserId`
- `RoleId`
- `PermissionId`
- `PatientId`
- `VisitId`
- `EncounterId`
- `ServiceId`
- `BrigadeId`
- `FormTemplateId`
- `DocumentTemplateId`
- `DeviceId`
- `SyncBatchId`

Sin embargo, antes de tocar migraciones, debe decidirse que relaciones seran FKs reales y que relaciones se conservaran como referencias logicas por razones de sincronizacion offline, auditoria, importacion o tolerancia a datos historicos.

---

## 3. Clasificacion propuesta de relaciones

### 3.1 Relaciones que probablemente deben ser FKs fuertes

Estas relaciones son candidatas a constraints reales porque representan ownership o composicion operativa directa.

| Entidad dependiente | Campo | Entidad principal | Motivo |
|---|---|---|---|
| `Role` | `OrganizationId` | `Organization` | Los roles pertenecen a una organizacion. |
| `User` | `OrganizationId` | `Organization` | Los usuarios pertenecen a una organizacion. |
| `Permission` | N/A | N/A | Catalogo global, no tenant-owned. |
| `RolePermission` | `RoleId` | `Role` | Grant pertenece a un rol. |
| `RolePermission` | `PermissionId` | `Permission` | Grant apunta a permiso existente. |
| `UserRole` | `OrganizationId` | `Organization` | Asignacion ocurre dentro del tenant. |
| `UserRole` | `UserId` | `User` | Asignacion pertenece a usuario. |
| `UserRole` | `RoleId` | `Role` | Asignacion apunta a rol. |
| `Service` | `OrganizationId` | `Organization` | Servicio pertenece a organizacion. |
| `Brigade` | `OrganizationId` | `Organization` | Brigada pertenece a organizacion. |
| `BrigadeService` | `BrigadeId` | `Brigade` | Servicio habilitado pertenece a brigada. |
| `BrigadeService` | `ServiceId` | `Service` | Servicio habilitado apunta a servicio. |
| `PatientVisit` | `PatientId` | `Patient` | Visita pertenece a paciente. |
| `PatientVisit` | `BrigadeId` | `Brigade` | Visita ocurre en brigada. |
| `ServiceEncounter` | `VisitId` | `PatientVisit` | Encuentro pertenece a visita. |
| `ServiceEncounter` | `ServiceId` | `Service` | Encuentro apunta a servicio. |
| `FormTemplate` | `ServiceId` | `Service` | Template pertenece al servicio. |
| `FormResponse` | `FormTemplateId` | `FormTemplate` | Respuesta usa un template. |
| `FormResponse` | `EncounterId` | `ServiceEncounter` | Respuesta pertenece a encuentro. |
| `SyncEvent` | `SyncBatchId` | `SyncBatch` | Evento pertenece a lote de sincronizacion. |

### 3.2 Relaciones que requieren decision antes de FK

Estas relaciones pueden ser reales, pero deben revisarse por offline, auditoria o datos historicos.

| Campo | Motivo de cautela |
|---|---|
| `CreatedByUserId` | Puede referir usuarios desactivados, borrados o migrados. |
| `UpdatedByUserId` | Puede referir usuarios historicos. |
| `DeletedByUserId` | Puede referir usuarios historicos. |
| `SignedByUserId` | Puede existir firma aunque el usuario cambie o se desactive. |
| `ProviderUserId` | Puede ser externo, voluntario o dato historico. |
| `DeviceId` | Puede haber eventos offline de dispositivos revocados o no sincronizados. |
| `ActorUserId` en auditoria | No debe romper auditoria si el usuario ya no existe. |
| `EntityId` en auditoria | Debe tolerar entidades historicas o borradas. |
| `AiRequestLog.RequestedByUserId` | Requiere politica de retencion antes de FK fuerte. |

### 3.3 Relaciones que probablemente deben permanecer logicas

Estas referencias no deben volverse FKs en P2 sin analisis adicional:

| Area | Motivo |
|---|---|
| Auditoria generica | Debe sobrevivir aunque entidades se borren. |
| Crypto integrity records | Puede auditar entidades ya no presentes. |
| Export jobs historicos | No deben romperse por limpieza operacional. |
| AI request logs | Deben analizarse por privacidad, retencion y PII. |

---

## 4. Delete behavior recomendado

Regla general: evitar cascades destructivos.

### 4.1 Recomendacion base

Usar preferentemente:

- `DeleteBehavior.Restrict`
- `DeleteBehavior.NoAction`

Motivo:

- El sistema usa soft delete.
- Hay datos clinicos y operativos sensibles.
- No debe borrarse en cascada informacion de pacientes, visitas, formularios, consentimientos, auditoria o documentos.
- Las eliminaciones reales deben ser excepcionales y controladas.

### 4.2 Cascades a evitar

Evitar cascades desde:

- `Organization`
- `Patient`
- `Brigade`
- `Service`
- `User`
- `Role`
- `FormTemplate`
- `DocumentTemplate`

### 4.3 Posibles cascades controlados

Solo considerar cascades en tablas puramente join o dependientes tecnicas, por ejemplo:

- `RolePermission` desde `Role`
- `RolePermission` desde `Permission`
- `UserRole` desde `User` o `Role`

Aun asi, por auditoria, puede ser mejor restringir y usar desactivacion/soft delete.

---

## 5. Riesgos actuales si no se agregan FKs

- Datos huerfanos.
- Asignaciones de rol hacia roles inexistentes.
- Respuestas de formulario apuntando a encuentros inexistentes.
- Encuentros apuntando a visitas inexistentes.
- Servicios de brigada duplicados o apuntando a servicios borrados.
- Menor confianza en reportes.
- Mayor complejidad de limpieza de datos.
- Riesgo de inconsistencias en sincronizacion offline.
- Riesgo de que el frontend o clientes moviles oculten errores de integridad.

---

## 6. Riesgos de agregar FKs sin planeacion

- Romper migraciones si ya hay datos inconsistentes.
- Bloquear sincronizacion offline por orden de llegada.
- Romper auditoria historica.
- Impedir carga de datos parciales.
- Generar cascades peligrosos.
- Crear dependencias ciclicas.
- Aumentar complejidad de seeds.
- Romper pruebas existentes.

---

## 7. Plan P2 recomendado

### P2-02

Documento de findings y estrategia de integridad relacional.

### P2-03

Contrato EF para ausencia de cascades peligrosos.

No requiere migracion.

### P2-04

Contrato EF/SQL para foreign key inventory esperado.

Puede iniciar como test/documentacion que enumere candidatos, no necesariamente como migracion.

### P2-05

Primer paquete de FKs fuertes para seguridad/core:

- RolePermission -> Role
- RolePermission -> Permission
- UserRole -> User
- UserRole -> Role
- UserRole -> Organization
- Role -> Organization
- User -> Organization
- Service -> Organization

### P2-06

Segundo paquete de FKs para brigadas:

- Brigade -> Organization
- BrigadeService -> Brigade
- BrigadeService -> Service
- Community -> Organization
- MobileUnit -> Organization

### P2-07

Tercer paquete de FKs para clinica:

- Patient -> Organization
- PatientVisit -> Patient
- PatientVisit -> Brigade
- ServiceEncounter -> PatientVisit
- ServiceEncounter -> Service
- MedicalReferral -> Patient
- MedicalReferral -> ServiceEncounter
- MedicationDelivery -> Patient
- MedicationDelivery -> ServiceEncounter

### P2-08

Cuarto paquete de FKs para forms/documents/sync:

- FormTemplate -> Service
- FormResponse -> FormTemplate
- FormResponse -> ServiceEncounter
- DocumentSignature -> DocumentTemplate
- MediaRelease -> Patient
- SyncEvent -> SyncBatch

### P2-09

SQL deployment baseline refresh.

### P2-10

Data integrity release to main.

---

## 8. Regla para migraciones P2

Cada PR con migracion debe incluir:

- Cambio EF.
- Migration generada.
- SQL deployment script actualizado.
- Tests de modelo.
- Tests de SQL script si aplica.
- Validacion `dotnet build`.
- Validacion `dotnet test`.
- Validacion `scripts/verify-no-mojibake.ps1`.
- Revision de Codex antes de merge.

---

## 9. Regla para datos existentes

Antes de agregar FKs en ambientes con datos reales:

- Ejecutar queries de orphan detection.
- Documentar resultados.
- Limpiar datos si aplica.
- Aplicar migracion despues de limpieza.
- Tener rollback plan.
- No aplicar cascades destructivos.

---

## 10. Siguiente PR recomendado

P2-03: agregar contratos EF para prohibir cascades peligrosos y documentar que el modelo debe usar `Restrict` o `NoAction` en relaciones criticas.

Ese PR debe preparar el terreno antes de crear foreign keys reales.