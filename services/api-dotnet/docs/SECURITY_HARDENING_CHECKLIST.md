# Security Hardening Checklist

Este documento define el checklist de seguridad técnica para evolucionar el backend de Cáritas Brigadas de Salud desde MVP local hacia un sistema preparado para ambientes reales.

## 1. Principio base

El objetivo no es decir que el sistema tiene cero vulnerabilidades de forma absoluta.

El objetivo correcto es:

- Reducir superficie de ataque.
- Aplicar controles preventivos.
- Validar seguridad de forma continua.
- Tener monitoreo y trazabilidad.
- Corregir vulnerabilidades antes de producción.
- Documentar riesgos aceptados.
- Evitar datos reales hasta tener controles suficientes.

## 2. Estado actual del MVP

El MVP local ya tiene:

- Separación por capas.
- Respuestas estandarizadas.
- TraceId/correlationId.
- HTTPS local.
- CORS configurable.
- Rate limiting configurable.
- Roles.
- Permisos.
- Seed de seguridad.
- Validaciones básicas por request.
- Entity Framework Core.
- SQL Server LocalDB.
- Smoke test local.
- Documentación técnica.
- Endpoints principales versionados bajo /api/v1.

Pendiente para producción:

- Autenticación real.
- Autorización estricta por permisos.
- Cifrado de datos sensibles.
- Auditoría formal.
- Protección avanzada contra abuso.
- Monitoreo.
- Backups.
- Gestión formal de secretos.
- Pruebas de seguridad.
- Revisión legal.
- Políticas de retención.

## 3. Autenticación

Requerido antes de producción:

- Implementar autenticación real.
- Usar JWT, OAuth2/OIDC o proveedor de identidad.
- No aceptar usuarios anónimos en endpoints sensibles.
- Separar usuarios internos, voluntarios, estudiantes y administradores.
- Soportar revocación o expiración de sesión.
- Definir expiración de tokens.
- Definir refresh tokens si aplica.
- Bloquear usuarios inactivos.
- Registrar último acceso si aplica.
- Evitar credenciales hardcodeadas.

Opciones futuras:

- Microsoft Entra ID.
- Auth0.
- AWS Cognito.
- Azure AD B2C.
- IdentityServer.
- Supabase Auth si se integra frontend externo.
- Autenticación institucional de Cáritas si existe.

## 4. Autorización

Requerido:

- Aplicar autorización por rol y permiso.
- No depender solo del frontend.
- Validar organización en cada operación.
- Evitar acceso cruzado entre organizaciones.
- Evitar que un usuario de una organización consulte datos de otra.
- Agregar policies por permiso.
- Proteger endpoints administrativos.
- Proteger endpoints clínicos.
- Proteger reportes.
- Proteger consentimientos.
- Proteger auditoría.

Permisos sugeridos:

- organizations.read
- organizations.write
- users.read
- users.write
- roles.read
- roles.assign
- services.read
- services.seed
- brigades.read
- brigades.write
- patients.read
- patients.write
- visits.read
- visits.write
- encounters.read
- encounters.write
- forms.read
- forms.write
- consents.read
- consents.write
- reports.read
- reports.export
- sync.write
- audit.read

## 5. Validación de entrada

Requerido:

- Validar DTOs con DataAnnotations.
- Validar campos obligatorios.
- Validar longitudes máximas.
- Validar enums.
- Validar fechas.
- Validar GUIDs.
- Validar JSON.
- Rechazar payloads demasiado grandes.
- No confiar en datos del cliente.
- Normalizar strings.
- Evitar campos libres sin límite.

Casos sensibles:

- CURP.
- Teléfono.
- Nombre del paciente.
- Dirección.
- Firma digital.
- Notas clínicas.
- Notas psicológicas.
- Consentimientos.
- Payloads offline.

## 6. SQL Injection

Estado actual:

- El uso de Entity Framework Core reduce riesgo de SQL injection si se usan LINQ y parámetros.
- Los endpoints creados usan principalmente LINQ parametrizado.
- El endpoint de auditoría usa SQL directo, pero con parámetros para valores variables.

Requerido:

- No concatenar valores del usuario en SQL.
- Usar parámetros siempre.
- Validar nombres de columnas y tablas si se usa SQL dinámico.
- Mantener whitelist de tablas permitidas.
- Evitar raw SQL innecesario.
- Revisar cualquier FromSqlRaw.
- Preferir FromSqlInterpolated cuando aplique.
- No exponer filtros arbitrarios sin sanitización.
- No permitir orderBy libre sin whitelist.

## 7. XSS

Aunque el backend es API, debe prevenir persistencia de contenido peligroso.

Requerido:

- Tratar campos de texto como datos, no como HTML.
- No renderizar HTML desde respuestas sin sanitizar en frontend.
- Validar o sanitizar notas libres si después se muestran en UI.
- Escapar en frontend.
- Proteger campos como:
  - clinicalNotes
  - recommendations
  - documentTextSnapshot
  - responseJson
  - notesAdmin

## 8. CSRF

Si se usa autenticación por cookies:

- Implementar protección CSRF.
- Configurar SameSite.
- Usar tokens anti-CSRF.
- Validar origen.

Si se usa JWT en Authorization header:

- Riesgo CSRF menor.
- Mantener CORS estricto.
- No guardar tokens en lugares inseguros.

## 9. CORS

Estado actual:

- CORS local configurable.

Requerido para producción:

- No usar wildcard.
- Permitir solo dominios oficiales.
- Separar dominios de dev, staging y producción.
- No permitir cualquier origen.
- Revisar headers permitidos.
- Revisar métodos permitidos.
- No abrir credentials si no es necesario.

## 10. HTTPS

Estado actual:

- HTTPS local en desarrollo.

Requerido:

- HTTPS obligatorio en producción.
- HSTS.
- Certificados válidos.
- Redirección HTTP a HTTPS.
- TLS moderno.
- No aceptar protocolos inseguros.
- No usar certificados self-signed en producción.

## 11. Rate limiting y protección anti abuso

Estado actual:

- Rate limiting configurable.

Requerido:

- Activar rate limiting por ambiente.
- Limitar endpoints de escritura.
- Limitar endpoints de login cuando exista.
- Limitar sync batches.
- Limitar creación masiva de pacientes.
- Limitar exportaciones.
- Limitar reportes pesados.
- Agregar protección por IP y usuario.
- Agregar cuotas por organización si aplica.

Consideraciones anti DDoS:

- La app no debe intentar resolver DDoS sola.
- Usar protección de infraestructura:
  - Cloudflare
  - Azure Front Door
  - AWS WAF
  - API Gateway
  - Load balancer con reglas
  - WAF administrado
- Configurar límites de payload.
- Configurar timeouts.
- Configurar max request body size.
- Rechazar payloads offline excesivos.

## 12. Protección de datos sensibles

Datos sensibles actuales o futuros:

- Nombre del paciente.
- Fecha de nacimiento.
- Edad.
- Sexo.
- CURP.
- Teléfono.
- Dirección.
- Comunidad.
- Notas clínicas.
- Notas psicológicas.
- Consentimientos.
- Firma digital.
- Medicamentos.
- Referencias médicas.

Requerido:

- Clasificar datos.
- Minimizar datos recolectados.
- Cifrar datos sensibles en reposo.
- Cifrar backups.
- Cifrar tránsito.
- Restringir acceso por permiso.
- Evitar logs con datos sensibles.
- Evitar imprimir payloads completos.
- Definir retención.
- Definir eliminación lógica/física.
- Definir política de anonimización para analítica.

## 13. Logging seguro

Requerido:

- Logging estructurado.
- No registrar CURP.
- No registrar firma base64.
- No registrar notas clínicas completas.
- No registrar responseJson completo en producción.
- No registrar connection strings.
- No registrar secretos.
- Registrar traceId.
- Registrar endpoint.
- Registrar status code.
- Registrar duración.
- Registrar usuario cuando exista autenticación.
- Registrar organización.
- Registrar errores controlados.

## 14. Auditoría formal

Estado actual:

- Endpoint de lectura defensivo si existe tabla compatible.

Requerido:

- Crear entidad AuditLog formal.
- Registrar:
  - organizationId
  - userId
  - action
  - entityName
  - entityId
  - before/after si aplica
  - timestamp
  - correlationId
  - ip
  - userAgent
- Auditar:
  - creación de paciente
  - edición de paciente
  - creación de visita
  - creación de atención
  - creación de form response
  - creación de consentimiento
  - exportación de reportes
  - asignación de roles
  - cambios de permisos
  - sync batches

## 15. Manejo de errores

Estado actual:

- ApiErrorResponse estandarizado.

Requerido:

- No exponer stack traces en producción.
- No exponer detalles internos de SQL.
- No exponer nombres internos innecesarios.
- Mantener traceId para soporte.
- Devolver errores consistentes.
- Diferenciar 400, 401, 403, 404, 409, 429, 500.
- Registrar detalle completo solo en logs internos.

## 16. Secrets management

Prohibido:

- Connection strings productivas en código.
- Passwords en appsettings versionados.
- API keys en GitHub.
- Secretos en scripts.
- Certificados en repositorio.

Requerido:

- Variables de entorno.
- User Secrets solo en local.
- Azure Key Vault o AWS Secrets Manager en cloud.
- Rotación de secretos.
- Separación dev/staging/prod.
- Revisión de commits para secretos.

## 17. Base de datos

Requerido:

- Usuario de base de datos con mínimos privilegios.
- No usar sa.
- No usar cuenta admin desde API.
- Backups automáticos.
- Pruebas de restauración.
- Índices para consultas críticas.
- Migraciones revisadas.
- Separar ambientes.
- Cifrado en reposo.
- Monitoreo de conexiones.
- Timeouts.
- Plan de recuperación.

## 18. Migraciones

Buenas prácticas:

- No generar migraciones sin revisar.
- No aplicar migraciones destructivas sin respaldo.
- Revisar nombres de columnas.
- Revisar nullable vs non-nullable.
- Validar en staging antes de producción.
- Evitar cambios manuales no versionados en BD.
- Mantener migraciones en Git.

## 19. Archivos y firmas

Para firmas y documentos:

- No guardar archivos grandes directo en SQL si crece demasiado.
- Evaluar Blob Storage o File Storage.
- Guardar URL segura o identificador.
- Validar tamaño de firma.
- Validar tipo MIME.
- No aceptar cualquier data URL.
- Escanear archivos si después se aceptan PDFs o imágenes.
- Controlar acceso a documentos.
- Firmar URLs temporales si están en storage.

## 20. Sync offline

Riesgos:

- Duplicados.
- Reintentos.
- Conflictos.
- Payloads manipulados.
- Eventos fuera de orden.
- Dispositivo comprometido.
- Datos sensibles almacenados localmente.

Requerido:

- Idempotencia por clientBatchId.
- Persistencia de payload o eventos.
- SyncBatchItem.
- Estados:
  - Received
  - Processing
  - Completed
  - Failed
  - Conflict
- Validación por evento.
- Firma o hash de lote si aplica.
- Control de dispositivo.
- Cifrado local en app móvil.
- Resolución de conflictos.
- Reintentos seguros.

## 21. Reportes y exportaciones

Requerido:

- Proteger reportes con permisos.
- Auditar exportaciones.
- Limitar volumen exportado.
- Evitar exportar datos sensibles sin autorización.
- Agregar filtros.
- Agregar paginación.
- Para CSV:
  - Escapar valores.
  - Evitar CSV injection.
  - Prevenir fórmulas maliciosas si se exportan campos libres.
- Para XLSX:
  - Usar librería mantenida.
  - Validar tamaño.
  - Evitar macros.

## 22. CSV Injection

Cuando se exporten campos libres a CSV, proteger valores que empiecen con:

- =
- +
- -
- @

Mitigación:

- Prefijar apostrofe.
- Escapar correctamente.
- Evitar incluir campos libres sin sanitización.
- Validar con Excel/LibreOffice.

El CSV actual de summary exporta métricas controladas, por lo que el riesgo es bajo.

## 23. Headers de seguridad

Agregar en producción:

- Strict-Transport-Security
- X-Content-Type-Options
- X-Frame-Options o CSP frame-ancestors
- Content-Security-Policy si sirve contenido web
- Referrer-Policy
- Permissions-Policy
- Cache-Control para datos sensibles

## 24. OWASP API Security Top 10

Validar contra:

- Broken Object Level Authorization
- Broken Authentication
- Broken Object Property Level Authorization
- Unrestricted Resource Consumption
- Broken Function Level Authorization
- Unrestricted Access to Sensitive Business Flows
- Server Side Request Forgery
- Security Misconfiguration
- Improper Inventory Management
- Unsafe Consumption of APIs

## 25. Ambientes

Separar:

- Local
- Development
- Staging
- Production

Cada ambiente debe tener:

- Base de datos propia.
- Secrets propios.
- Logging propio.
- CORS propio.
- URLs propias.
- Configuración de seguridad propia.

## 26. Checklist previo a producción

Antes de producción debe cumplirse:

- Autenticación implementada.
- Autorización por permisos implementada.
- HTTPS obligatorio.
- CORS restringido.
- Rate limiting activo.
- Logs sin datos sensibles.
- Auditoría formal.
- Backups configurados.
- Prueba de restauración.
- Secrets fuera del repo.
- Revisión de migraciones.
- Revisión de OWASP.
- Smoke test exitoso.
- Pruebas de carga básicas.
- Pruebas de seguridad.
- Revisión legal de aviso de privacidad.
- Validación con usuarios reales.
- Documentación de operación.
- Plan de respuesta a incidentes.

## 27. No usar datos reales todavía

Hasta completar controles de seguridad y privacidad:

- No capturar pacientes reales.
- No capturar CURP real.
- No capturar firmas reales.
- No capturar notas clínicas reales.
- No capturar datos psicológicos reales.
- No usar documentos legales reales sin revisión.

Usar datos ficticios para desarrollo.

## 28. Prioridad de implementación recomendada

Orden sugerido:

1. Autenticación.
2. Autorización por permisos.
3. Auditoría formal.
4. Cifrado/protección de datos sensibles.
5. Rate limiting activo.
6. Paginación y filtros.
7. Hardening de reportes/exportaciones.
8. Sync offline robusto.
9. Monitoreo y logging estructurado.
10. Backups y recuperación.
11. Pruebas de seguridad.
12. Preparación cloud.

## 29. Criterio profesional

El proyecto debe comunicar seguridad con precisión.

Correcto:

- Sistema diseñado con prácticas de seguridad.
- Sistema preparado para endurecimiento progresivo.
- MVP local sin datos reales.
- Ruta clara hacia producción segura.

Incorrecto:

- Prometer cero vulnerabilidades.
- Prometer inmunidad a DDoS.
- Prometer cumplimiento legal sin revisión.
- Usar datos reales en ambiente local.
- Exponer endpoints sensibles sin autenticación.
Estado de enforcement actual

El backend ya cuenta con enforcement progresivo de autorización por policies.

Protegidos actualmente:

Reports.
Audit logs.
Patients.
Patient visits.
Service encounters.
Form templates.
Form responses.
Consent documents.
Services.
Communities.
Mobile units.
Brigades.
Brigade services.
Organizations.
Users.
Security.
Sync batches.

Documento específico:

docs/ENFORCED_ENDPOINT_SECURITY_STATUS.md

Pendiente crítico:

Validación sistemática de acceso por organización.
Autenticación productiva vía JWT/OIDC.
Auditoría formal de escritura.

Tests de integración 401/403/200 sobre endpoints reales.
