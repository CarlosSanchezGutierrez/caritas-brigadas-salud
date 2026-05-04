# Base de datos local — SQL Server

Esta guía explica cómo preparar la base de datos local del backend.

## 1. Requisitos

Debes tener disponible una instancia local de SQL Server.

Opciones válidas:

- SQL Server Developer
- SQL Server Express
- SQL Server LocalDB
- SQL Server en Docker
- SQL Server en una VM local

Para este proyecto, la cadena de conexión local de ejemplo es:

```json
"Server=localhost;Database=CaritasBrigadas_Local;Trusted_Connection=True;TrustServerCertificate=True;"
2. Crear configuración local

Desde services/api-dotnet:

.\scripts\setup-local-appsettings.ps1

Esto crea:

src/Caritas.Brigadas.Api/appsettings.Local.json

Ese archivo no debe subirse al repositorio.

3. Revisar cadena de conexión

Abrir:

src/Caritas.Brigadas.Api/appsettings.Local.json

Verificar:

{
  "ConnectionStrings": {
    "SqlServer": "Server=localhost;Database=CaritasBrigadas_Local;Trusted_Connection=True;TrustServerCertificate=True;"
  }
}
4. Aplicar migraciones

Desde services/api-dotnet:

.\scripts\update-local-database.ps1

Esto ejecuta:

dotnet ef database update

usando:

CaritasDbContext
Caritas.Brigadas.Infrastructure
Caritas.Brigadas.Api como startup project
5. Generar script SQL idempotente

Desde services/api-dotnet:

.\scripts\generate-idempotent-sql.ps1

Salida esperada:

database/migrations/sqlserver/latest_idempotent.sql
6. Reglas
No subir appsettings.Local.json.
No subir .bak, .mdf, .ldf ni datos reales.
No usar datos reales de pacientes en desarrollo.
No conectar frontend, iOS ni Android directo a SQL Server.
Toda lectura/escritura debe pasar por la API.
La base local debe usarse solo con datos sintéticos.
