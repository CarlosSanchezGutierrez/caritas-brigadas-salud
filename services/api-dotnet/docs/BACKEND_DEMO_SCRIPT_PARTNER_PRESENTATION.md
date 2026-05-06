# Backend Demo Script for Partner Presentation

Este documento define un guion práctico para demostrar el backend local del MVP de Cáritas Brigadas de Salud ante socio formador, profesores o equipo técnico.

## 1. Objetivo de la demo

Demostrar que el backend ya puede modelar el flujo operativo principal de una brigada de salud:

1. Organización.
2. Usuarios.
3. Seguridad.
4. Servicios.
5. Brigadas.
6. Pacientes.
7. Visitas.
8. Atenciones.
9. Formularios.
10. Consentimientos.
11. Reportes.
12. Auditoría.

La demo debe comunicar:

```text
Tenemos una base backend funcional, protegida y auditable para operar brigadas de salud.
2. Mensaje inicial sugerido
Este MVP backend no es todavía un sistema productivo ni debe usarse con datos reales. Es una base técnica local validable que demuestra cómo podría digitalizarse la operación de brigadas de salud: desde la creación de una brigada, el registro de pacientes, la atención por servicio, la captura de formularios y consentimientos, hasta reportes y auditoría.
3. Qué no prometer

No decir:

Ya está listo para producción.
Ya cumple legalmente.
Ya tiene cero vulnerabilidades.
Ya está listo para datos reales.
Ya está conectado a autenticación institucional.
Ya tiene operación offline completa.

Sí decir:

Ya existe una base técnica modular, protegida localmente y preparada para evolucionar hacia producción.
4. Preparación antes de la demo

Desde backend:

cd C:\Users\Skere\Documents\GitHub\caritas-brigadas-salud\services\api-dotnet

$env:ASPNETCORE_ENVIRONMENT = "Development"
$env:DOTNET_ENVIRONMENT = "Development"
$env:Authentication__Mode = "Development"
$env:ConnectionStrings__SqlServer = "Server=(localdb)\MSSQLLocalDB;Database=CaritasBrigadas_Local;Trusted_Connection=True;TrustServerCertificate=True;"

Aplicar migraciones:

dotnet tool restore

dotnet tool run dotnet-ef database update `
  --context CaritasDbContext `
  --project src/Caritas.Brigadas.Infrastructure/Caritas.Brigadas.Infrastructure.csproj `
  --startup-project src/Caritas.Brigadas.Api/Caritas.Brigadas.Api.csproj `
  --connection "Server=(localdb)\MSSQLLocalDB;Database=CaritasBrigadas_Local;Trusted_Connection=True;TrustServerCertificate=True;"

Validar build y tests:

dotnet build Caritas.Brigadas.sln
dotnet test Caritas.Brigadas.sln

Correr API:

dotnet run `
  --project src/Caritas.Brigadas.Api/Caritas.Brigadas.Api.csproj `
  --no-launch-profile `
  --urls "https://localhost:7044;http://localhost:5031"
5. Abrir Swagger

Abrir:

https://localhost:7044/swagger

Explicación sugerida:

Swagger nos permite revisar todos los endpoints del backend. Aquí se puede ver que la API está versionada bajo /api/v1 y que los módulos están separados por función: organizaciones, usuarios, servicios, brigadas, pacientes, formularios, reportes, sincronización y auditoría.
6. Health check

Comando:

$baseUrl = "https://localhost:7044"

curl.exe -k -sS "$baseUrl/api/v1/health"

Explicación:

Primero validamos que el servicio está vivo. El health check responde success:true, lo cual confirma que la API está corriendo correctamente.
7. Variables de demo
$baseUrl = "https://localhost:7044"
$organizationId = "4df92032-4a1c-4cf2-b48f-15b570cd073a"
$userId = "76279895-817d-47d2-b5c2-2a1e306db4f9"

Headers de desarrollo:

$devHeaders = @(
  "-H", "X-Dev-User-Id: $userId",
  "-H", "X-Dev-Organization-Id: $organizationId",
  "-H", "X-Dev-Roles: SUPER_ADMIN",
  "-H", "X-Dev-Name: Smoke Test User",
  "-H", "X-Dev-Email: smoke.test@caritas.local"
)

Explicación:

En local usamos headers de desarrollo para simular un usuario autenticado. En producción esto se reemplazaría por JWT/OIDC con un proveedor de identidad real.
8. Demostrar autorización
Sin autenticación
curl.exe -k -i "$baseUrl/api/v1/organizations/$organizationId/reports/summary"

Resultado esperado:

401 Unauthorized

Explicación:

Los endpoints principales ya no están abiertos. Si alguien intenta entrar sin autenticación, la API lo rechaza.
Con autenticación local
curl.exe -k -sS "$baseUrl/api/v1/organizations/$organizationId/reports/summary" `
  -H "X-Dev-User-Id: $userId" `
  -H "X-Dev-Organization-Id: $organizationId" `
  -H "X-Dev-Roles: SUPER_ADMIN"

Resultado esperado:

success:true

Explicación:

Con usuario autenticado y permisos suficientes, el endpoint responde correctamente.
9. Ejecutar smoke test completo

Desde raíz:

cd C:\Users\Skere\Documents\GitHub\caritas-brigadas-salud

powershell -ExecutionPolicy Bypass -File .\services\api-dotnet\scripts\smoke-test-local.ps1

Resultado esperado:

SMOKE TEST COMPLETED SUCCESSFULLY

Explicación:

Este script prueba el flujo completo del MVP local. Crea y consulta los elementos principales: organización, usuarios, seguridad, servicios, brigada, paciente, visita, atención, formulario, consentimiento, reportes, exportación CSV, sync batch y auditoría.
10. Explicar flujo de negocio

Guion:

El flujo central del sistema es: Cáritas organiza una brigada en una comunidad, define qué servicios estarán disponibles, registra pacientes, registra visitas, abre atenciones por servicio, captura formularios clínicos o administrativos, guarda consentimiento o aviso de privacidad, y al final puede consultar reportes y auditoría.
11. Explicar módulos principales
Organizations
Representa la institución o entidad operativa, en este caso Cáritas de Monterrey.
Users
Representa colaboradores, coordinadores, estudiantes, personal de recepción, prestadores de salud y administradores.
Security
Controla roles y permisos. No usamos solamente 'doctor', sino HealthProvider para incluir médicos, psicólogos, nutriólogos, optometristas y otros perfiles de atención.
Services
Define los servicios disponibles: medicina general, odontología, optometría, nutrición, psicología, entrega de medicamentos, referencia médica y trabajo social.
Brigades
Representa una jornada de atención en una comunidad, fecha y ubicación específica.
Patients
Permite registrar pacientes con datos básicos, manteniendo el principio de mínima información necesaria.
Patient Visits
Registra que un paciente acudió a una brigada específica.
Service Encounters
Registra una atención concreta dentro de la visita, por ejemplo medicina general.
Form Templates y Form Responses
Permiten capturar formularios versionados por servicio. Esto evita hardcodear formularios en frontend y facilita evolucionar el sistema.
Consent Documents
Guarda consentimientos o avisos de privacidad con versión, snapshot del texto aceptado y evidencia de firma.
Reports
Entrega un resumen operativo en JSON y CSV.
Audit Logs
Permite registrar acciones sensibles: creación de pacientes, visitas, atenciones, consentimientos, exportaciones y cambios operativos.
12. Demostrar reporte JSON
curl.exe -k -sS "$baseUrl/api/v1/organizations/$organizationId/reports/summary" `
  -H "X-Dev-User-Id: $userId" `
  -H "X-Dev-Organization-Id: $organizationId" `
  -H "X-Dev-Roles: SUPER_ADMIN"

Explicación:

Este reporte resume el estado operativo de la organización: usuarios, roles, servicios, brigadas, pacientes, visitas, atenciones, formularios, consentimientos y registros clínicos.
13. Demostrar exportación CSV
curl.exe -k -L -o ".\report-summary.csv" "$baseUrl/api/v1/organizations/$organizationId/reports/summary.csv" `
  -H "X-Dev-User-Id: $userId" `
  -H "X-Dev-Organization-Id: $organizationId" `
  -H "X-Dev-Roles: SUPER_ADMIN"

Get-Content ".\report-summary.csv"
Remove-Item ".\report-summary.csv" -Force -ErrorAction SilentlyContinue

Explicación:

La exportación permite sacar datos agregados para análisis, seguimiento operativo o reportes internos. La exportación queda auditada.
14. Demostrar auditoría
curl.exe -k -sS "$baseUrl/api/v1/organizations/$organizationId/audit-logs" `
  -H "X-Dev-User-Id: $userId" `
  -H "X-Dev-Organization-Id: $organizationId" `
  -H "X-Dev-Roles: SUPER_ADMIN"

Explicación:

La auditoría permite saber qué acciones sensibles ocurrieron, cuándo, en qué organización y por qué usuario. Esto es clave para trazabilidad, seguridad y operación responsable.
15. Puntos técnicos fuertes para mencionar
Arquitectura por capas.
Separación Domain/Application/Infrastructure/Contracts/API.
Entity Framework Core.
SQL Server.
Migraciones.
DTOs.
Repositorios.
Swagger.
Smoke test automatizado.
Autorización por policies.
Validación por organización.
Skeleton JWT Bearer.
Auditoría formal.
Reportes JSON y CSV.
Base para modo offline mediante sync batches.
16. Puntos de negocio fuertes
Reduce captura manual.
Centraliza información de brigadas.
Permite trazabilidad.
Permite reportes.
Permite auditoría.
Facilita escalar a web, móvil y cloud.
Permite adaptar servicios por brigada.
Permite operar con formularios versionados.
Ayuda a profesionalizar la operación social.
17. Riesgos y límites a declarar
Todavía no debe usarse con datos reales.
Falta autenticación productiva.
Falta revisión legal final.
Falta frontend.
Falta despliegue cloud.
Falta hardening completo.
Falta validación UX con usuarios reales.
Falta operación offline completa.
18. Cierre sugerido
La intención de esta etapa no fue crear una interfaz visual final, sino construir una base backend sólida. Con esto ya podemos pasar a una demo frontend mínima, validar el flujo con usuarios reales y decidir qué módulos priorizar antes de producción.
19. Siguiente demo recomendada

La siguiente presentación debería mostrar:

Frontend mínimo conectado a este backend.

Pantallas sugeridas:

Dashboard.
Selección de brigada.
Registro de paciente.
Registro de visita.
Atención por servicio.
Formulario clínico.
Consentimiento.
Reporte.
Auditoría.
