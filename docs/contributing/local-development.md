# Local Development

## Objetivo

Levantar el proyecto localmente sin romper configuración, seguridad ni datos.

## Requisitos generales

- Git.
- GitHub CLI.
- PowerShell.
- .NET SDK configurado por global.json.
- Node.js compatible con el frontend.
- SQL Server o LocalDB para desarrollo backend.
- Docker Desktop opcional para validar imagen local.

## Backend

Ubicación:

```powershell
cd services/api-dotnet
```

Comandos principales:

```powershell
dotnet restore Caritas.Brigadas.sln
dotnet build Caritas.Brigadas.sln /p:TreatWarningsAsErrors=true
dotnet test Caritas.Brigadas.sln /p:TreatWarningsAsErrors=true
```

## Frontend

Ubicación:

```powershell
cd apps/web-next
```

Comandos principales:

```powershell
npm ci
npm run typecheck
npm run build
npm run test:e2e:list
```

## Validación local integral

Desde la raíz:

```powershell
powershell -ExecutionPolicy Bypass -File scripts/verify-local.ps1 -SkipGitClean
powershell -ExecutionPolicy Bypass -File scripts/security-smoke-local.ps1
```

## Variables y secretos

No commitear secretos.

Usar archivos example para documentar forma, no valores reales.

Los secretos reales deben vivir en GitHub Secrets, Azure Key Vault o mecanismo institucional equivalente.

## Datos reales

No usar datos reales de pacientes en desarrollo local, tests, demos o screenshots.

## Flujo Git

```powershell
git switch develop
git fetch origin
git reset --hard origin/develop
git switch -c feature/nombre-claro
```

Después:

- Cambiar código.
- Validar local.
- Commit.
- Push a rama.
- Abrir Pull Request.
