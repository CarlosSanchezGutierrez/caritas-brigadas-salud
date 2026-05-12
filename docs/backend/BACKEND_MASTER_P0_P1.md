# Backend Master Documentation v1 — P0/P1 Baseline

Proyecto: Caritas Brigadas de Salud  
Repositorio: caritas-brigadas-salud  
Estado: Backend P0/P1 promovido a main  
Ramas base: main y develop alineadas  
Stack backend: ASP.NET Core, .NET, Entity Framework Core, SQL Server, GitHub Actions, Docker, Next.js web app como cliente auxiliar actual

---

## 1. Objetivo del backend

El backend de Caritas Brigadas de Salud centraliza la operacion digital de brigadas de salud para organizaciones, usuarios, roles, permisos, servicios, brigadas, pacientes, visitas, respuestas de formularios, consentimientos, sincronizacion, reportes y auditoria.

El objetivo tecnico actual no es solo tener endpoints funcionando, sino tener una base profesional para servicio social universitario, continuidad entre generaciones de alumnos y crecimiento futuro hacia clientes web, iOS y Android.

---

## 2. Principios tecnicos del proyecto

El backend se esta construyendo bajo estos principios:

- Cero deuda tecnica innecesaria.
- Seguridad desde el diseno.
- Separacion por capas.
- Trazabilidad de cambios.
- Auditoria de operaciones sensibles.
- Validacion por pruebas.
- Releases mediante pull requests.
- Proteccion de ramas.
- No pushes directos a main/develop.
- PRs pequenos y revisables.
- UTF-8 controlado.
- Contratos de autorizacion automatizados.
- SQL Server como base principal.
- Preparacion para despliegue real detras de proxy/load balancer.
- Preparacion futura para observabilidad, movilidad offline, data analytics y LLM gateway.

---

## 3. Arquitectura actual de solucion

La solucion backend esta organizada en capas principales:

### 3.1 Domain

Contiene entidades, conceptos centrales y reglas que no deben depender de infraestructura externa.

### 3.2 Application

Contiene contratos de aplicacion, codigos de permisos, codigos de roles, interfaces de repositorios y servicios transversales.

### 3.3 Infrastructure

Contiene persistencia, Entity Framework Core, migraciones, repositorios concretos, seeds de seguridad y acceso a base de datos.

### 3.4 Api

Contiene controllers, middlewares, configuracion de autenticacion/autorizacion, CORS, rate limiting, health checks, Swagger, headers de seguridad y pipeline HTTP.

### 3.5 Contracts

Contiene DTOs y modelos de request/response para mantener separacion entre API externa y modelo interno.

### 3.6 Tests

Incluye pruebas unitarias, pruebas de integracion, pruebas de contratos de seguridad, pruebas de autorizacion, pruebas de seed, pruebas de permisos, pruebas de roles y pruebas de auditoria.

---

## 4. P0 — Backend deployment/security baseline

P0 dejo una base operativa minima profesional para poder confiar en el backend.

### 4.1 Elementos cerrados en P0

- Build backend estable.
- Tests backend estables.
- Migraciones EF registradas.
- SQL deployment baseline actualizado.
- Docker image build gate.
- GitHub Actions Verify.
- Repository Security workflow.
- Dependency Review corregido.
- Forwarded headers antes de consumidores de IP/scheme.
- HSTS respetando HTTPS proxied.
- Rate limiting considerando IP real detras de proxy.
- Seguridad de headers.
- Health endpoints.
- Configuracion de entorno local.
- Proteccion contra configuraciones inseguras en produccion.
- Release P0 promovido a main.

### 4.2 Decisiones P0 importantes

- Usar PRs para cambios en ramas protegidas.
- Usar squash merges para evitar merge commits.
- Validar SQL deployment como baseline.
- Evitar que errores de infraestructura local bloqueen el listado de migraciones cuando SQL Server no esta disponible.
- Mantener main como rama de release.
- Mantener develop como rama de integracion.

---

## 5. P1 — Roles and permissions security baseline

P1 endurecio la parte critica de seguridad logica: roles, permisos, seeds, scoping y contratos de autorizacion.

### 5.1 P1-01 — GlobalOnly + cleanup de grants viejos

Se agrego `PermissionCodes.GlobalOnly`.

`organizations.write` fue clasificado como permiso global-only.

Se removio `organizations.write` del seed del rol tenant `ADMIN`.

Se agrego limpieza de grants viejos para que roles no super-admin no conserven permisos global-only si ya habian sido sembrados antes.

Motivo: quitar el permiso del mapa de seed no era suficiente para instalaciones existentes porque los registros en `role_permissions` podian quedarse vivos.

### 5.2 P1-02 — Clasificacion GlobalOnly / TenantScoped

Se agrego `PermissionCodes.TenantScoped`.

Se agregaron pruebas para asegurar:

- Todo permiso de `PermissionCodes.All` esta clasificado.
- `GlobalOnly` y `TenantScoped` no se empalman.
- `organizations.write` es global-only.
- `organizations.read` es tenant-scoped.

### 5.3 P1-03 — Guardrail contra asignacion de SUPER_ADMIN

Se bloqueo que un usuario tenant admin con `roles.assign` pueda asignar `SUPER_ADMIN`.

Solo un super-admin puede asignar `SUPER_ADMIN`.

Tambien se agrego compatibilidad con claims legacy de super-admin para evitar falsos bloqueos en tokens/principals existentes.

### 5.4 P1-04 — RoleCodes alineado con el seed

Se corrigio la divergencia entre `RoleCodes` y el modelo real sembrado.

Roles actuales:

- `SUPER_ADMIN`
- `ADMIN`
- `BRIGADE_COORDINATOR`
- `HEALTH_PROVIDER`
- `SERVICE_STUDENT`
- `AUDITOR`
- `DATA_ANALYST`

Se removieron del modelo activo roles no sembrados como:

- `ORGANIZATION_ADMIN`
- `COORDINATOR`
- `RECEPTION`
- `VIEWER`

El seed ahora usa constantes de `RoleCodes` en lugar de strings sueltos.

### 5.5 P1-05 — Controller authorization contracts

Se agregaron pruebas de contrato para controllers.

Reglas:

- Todo action HTTP debe tener decision explicita de autorizacion:
  - `[Authorize]`, o
  - `[AllowAnonymous]`.
- Todo policy usado por controllers debe apuntar a un miembro valido de `PermissionCodes`.
- Todo permiso usado por controllers debe existir en `PermissionCodes.All`.
- Todo action que use permisos global-only debe tener guardrail de super-admin en la misma accion.

`HealthController` fue marcado explicitamente como anonymous.

### 5.6 P1-06 — Seed endpoint contracts

Se agregaron contratos para endpoints `seed-defaults`.

Reglas:

- Security seed requiere `PermissionCodes.RolesAssign`.
- Services seed requiere `PermissionCodes.ServicesSeed`.
- Form templates seed requiere `PermissionCodes.FormTemplatesSeed`.
- Los permisos de seed deben permanecer tenant-scoped.

---

## 6. Roles actuales

| Rol | Codigo | Alcance esperado |
|---|---|---|
| Superadministrador institucional | `SUPER_ADMIN` | Control global/institucional |
| Administrador institucional | `ADMIN` | Administracion operativa tenant |
| Coordinador de brigada | `BRIGADE_COORDINATOR` | Coordinacion de brigadas y operacion |
| Prestador de servicio de salud | `HEALTH_PROVIDER` | Atencion medica/servicios |
| Estudiante prestador de servicio | `SERVICE_STUDENT` | Apoyo supervisado |
| Auditor | `AUDITOR` | Consulta, trazabilidad y cumplimiento |
| Analista de datos | `DATA_ANALYST` | Reportes y analisis agregado |

---

## 7. Permisos global-only

Actualmente:

- `organizations.write`

Regla: los permisos global-only no deben ser asignados a roles tenant. Si aparecen grants viejos, el seed debe limpiarlos de roles no super-admin.

---

## 8. Permisos tenant-scoped

Incluyen permisos de lectura/escritura por organizacion para:

- organizaciones
- usuarios
- roles
- servicios
- comunidades
- unidades moviles
- brigadas
- servicios de brigada
- pacientes
- visitas
- encuentros de servicio
- templates de formularios
- respuestas de formularios
- consentimientos
- reportes
- sincronizacion
- auditoria

---

## 9. Endpoints protegidos

La regla actual es que todo endpoint sensible debe tener autorizacion explicita.

Los endpoints health quedan como anonymous de forma intencional.

Los endpoints con `organizationId` deben respetar scoping por organizacion.

Los endpoints globales deben tener guardrails adicionales.

---

## 10. Seeds de seguridad

Los seeds de roles/permisos son sensibles porque mutan la configuracion base de seguridad.

Reglas actuales:

- No deben otorgar permisos global-only a roles tenant.
- Deben limpiar grants viejos global-only de roles no super-admin.
- Deben usar `RoleCodes`.
- Deben usar `PermissionCodes`.
- Los endpoints de seed deben tener permisos explicitos.

---

## 11. UTF-8 y mojibake policy

Se agrego `scripts/verify-no-mojibake.ps1`.

Objetivo:

- Detectar corrupcion de codificacion en archivos vivos del repositorio.
- Evitar texto corrupto por doble codificacion o mojibake.
- Mantener archivos en UTF-8 sin BOM.
- Evitar que codigo fuente, documentacion viva, SQL, scripts o CI contengan caracteres corruptos.

Politica:

- Escribir archivos con `[System.Text.UTF8Encoding]::new($false)`.
- Configurar PowerShell con code page 65001.
- Configurar Git con encoding UTF-8.
- No ignorar mojibake en codigo fuente, docs vivos, SQL, scripts o CI.
- Reportes historicos pueden excluirse si son evidencia antigua generada.

---

## 12. CI/CD y gates

Workflows activos relevantes:

- Verify.
- Repository Security.
- Dependency Review.
- Backend security and quality gate.
- Frontend security and quality gate.
- Database deployment baseline metadata gate.
- Deployment baseline metadata gate.
- Docker image build gate.
- Repository governance metadata gate.
- Testing baseline metadata gate.
- Supply chain baseline metadata gate.

Regla: ningun release se mergea con checks fallando o comentarios P1/P2 reales sin revisar.

---

## 13. Branching model

Ramas principales:

- `main`: release estable.
- `develop`: integracion estable.
- `feature/*`: trabajo tecnico.
- `fix/*`: hotfixes pequenos.
- `sync/*`: alineaciones excepcionales.
- `release/*`: promocion a main.

Reglas:

- No push directo a ramas protegidas.
- No merge commits en ramas protegidas.
- Squash merge.
- PRs pequenos.
- Cada PR debe tener validacion clara.

---

## 14. Estado actual

P0 y P1 estan cerrados y promovidos a main.

`main` y `develop` deben permanecer alineados despues del release P1.

---

## 15. Que NO significa este documento

Este documento no significa que el backend este terminado.

Significa que el baseline P0/P1 quedo estabilizado y documentado.

A partir de aqui deben continuar fases P2+ con PRs pequenos.

---

## 16. Siguientes fases recomendadas

Ver `BACKEND_ROADMAP.md`.