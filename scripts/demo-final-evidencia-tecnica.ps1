param(
    [string]$RunId = "",
    [string]$PrNumber = "",
    [switch]$SkipBuild,
    [switch]$SkipDocker,
    [switch]$SkipGitHubApi
)

$ErrorActionPreference = "Stop"

function Assert-ExitCode {
    param([Parameter(Mandatory = $true)][string]$Step)

    if ($LASTEXITCODE -ne 0) {
        throw "FAILED: $Step exited with code $LASTEXITCODE"
    }
}

function Add-Line {
    param([string]$Text = "")
    $script:ReportLines.Add($Text) | Out-Null
}

function Add-Section {
    param([string]$Title)

    Add-Line ""
    Add-Line "## $Title"
    Add-Line ""
}

function Add-Subsection {
    param([string]$Title)

    Add-Line ""
    Add-Line "### $Title"
    Add-Line ""
}

function Add-CommandOutput {
    param(
        [string]$Title,
        [string]$CommandText,
        [scriptblock]$Command,
        [switch]$AllowFailure
    )

    Add-Subsection $Title
    Add-Line "Comando:"
    Add-Line ""
    Add-Line "    $CommandText"
    Add-Line ""
    Add-Line "Salida:"
    Add-Line ""

    Write-Host ""
    Write-Host "=== $Title ==="
    Write-Host $CommandText

    $Output = & $Command 2>&1
    $ExitCode = $LASTEXITCODE

    if ($null -eq $Output -or $Output.Count -eq 0) {
        Add-Line "    [Sin salida]"
    }
    else {
        $Output | ForEach-Object {
            Add-Line ("    " + $_.ToString())
        }
    }

    Add-Line ""
    Add-Line "Exit code: $ExitCode"

    if ($ExitCode -ne 0 -and -not $AllowFailure) {
        throw "FAILED: $Title exited with code $ExitCode"
    }
}

function Get-GitFileCount {
    param([string]$Pattern)

    $Files = git ls-files $Pattern
    if ($LASTEXITCODE -ne 0) {
        $global:LASTEXITCODE = 0
        return 0
    }

    return @($Files).Count
}

function Write-Utf8NoBom {
    param(
        [Parameter(Mandatory = $true)][string]$Path,
        [Parameter(Mandatory = $true)][string]$Content
    )

    $Parent = Split-Path -Parent $Path

    if (-not [string]::IsNullOrWhiteSpace($Parent)) {
        [System.IO.Directory]::CreateDirectory($Parent) | Out-Null
    }

    $Utf8NoBom = New-Object System.Text.UTF8Encoding($false)
    [System.IO.File]::WriteAllText($Path, $Content, $Utf8NoBom)
}

$RepoRoot = (git rev-parse --show-toplevel).Trim()
Assert-ExitCode "git rev-parse"

Set-Location $RepoRoot

$ReportLines = New-Object System.Collections.Generic.List[string]
$ReportPath = Join-Path $RepoRoot "docs/demo/DEMO_FINAL_EVIDENCIA_TECNICA.md"

Add-Line "# Evidencia técnica para demo final"
Add-Line ""
Add-Line "Proyecto: Cáritas Brigadas de Salud"
Add-Line ""
Add-Line "Repositorio: caritas-brigadas-salud"
Add-Line ""
Add-Line "Fecha de generación: $(Get-Date -Format 'yyyy-MM-dd HH:mm:ss')"
Add-Line ""
Add-Line "Este documento resume evidencia técnica del repositorio para demo o presentación final."
Add-Line ""
Add-Line "Nota importante:"
Add-Line ""
Add-Line "- Este material muestra arquitectura, esfuerzo técnico, validaciones y alcance del backend."
Add-Line "- No representa aprobación productiva final."
Add-Line "- No declara conexión real al servidor SQL del Tec."
Add-Line "- No declara configuración final con el servidor de Cáritas."
Add-Line "- Backend production readiness remains BLOCKED_PENDING_REAL_EVIDENCE."
Add-Line "- SQL Server remains the operational source of truth."
Add-Line "- The API is the mandatory boundary."
Add-Line "- Mobile clients must not write directly to SQL Server."
Add-Line "- No client bypass of the API."

Add-Section "1. Estructura y magnitud del proyecto"

Add-Subsection "Conteo general de archivos"

$TotalFiles = @(git ls-files).Count
$CsFiles = Get-GitFileCount "*.cs"
$MdFiles = Get-GitFileCount "*.md"
$Ps1Files = Get-GitFileCount "*.ps1"
$JsonFiles = Get-GitFileCount "*.json"
$YamlFiles = @(git ls-files "*.yml" "*.yaml").Count

Add-Line "| Métrica | Conteo |"
Add-Line "|---|---:|"
Add-Line "| Archivos versionados totales | $TotalFiles |"
Add-Line "| Archivos C# | $CsFiles |"
Add-Line "| Archivos Markdown | $MdFiles |"
Add-Line "| Scripts PowerShell | $Ps1Files |"
Add-Line "| Archivos JSON | $JsonFiles |"
Add-Line "| Workflows/config YAML | $YamlFiles |"

Add-Subsection "Top 15 extensiones del repositorio"

Add-Line "| Extensión | Conteo |"
Add-Line "|---|---:|"

$ExtensionStats =
    git ls-files |
    ForEach-Object {
        $Extension = [System.IO.Path]::GetExtension($_)
        if ([string]::IsNullOrWhiteSpace($Extension)) {
            "[sin extension]"
        }
        else {
            $Extension.TrimStart(".").ToLowerInvariant()
        }
    } |
    Group-Object |
    Sort-Object Count -Descending |
    Select-Object -First 15

foreach ($Item in $ExtensionStats) {
    Add-Line "| $($Item.Name) | $($Item.Count) |"
}

Add-Subsection "Docs, verificadores y pruebas"

$VerifierCount = @(Get-ChildItem -Path (Join-Path $RepoRoot "scripts") -Filter "*.ps1" -File -ErrorAction SilentlyContinue).Count
$DocsCount = @(git ls-files "docs/**/*.md").Count
$TestFileCount = @(git ls-files "**/*Tests*.cs").Count
$TestProjectCount = @(git ls-files "*.Tests.csproj").Count

Add-Line "| Área | Conteo |"
Add-Line "|---|---:|"
Add-Line "| Verificadores/scripts .ps1 | $VerifierCount |"
Add-Line "| Documentos técnicos en docs/**/*.md | $DocsCount |"
Add-Line "| Archivos C# de pruebas *Tests*.cs | $TestFileCount |"
Add-Line "| Proyectos de pruebas .Tests.csproj | $TestProjectCount |"

Add-Subsection "Árbol del backend por capas"

$BackendSrc = Join-Path $RepoRoot "services/api-dotnet/src"

if (Test-Path $BackendSrc) {
    $BackendLayers = Get-ChildItem -Path $BackendSrc -Directory | Sort-Object Name

    Add-Line "| Capa / Proyecto | Ruta |"
    Add-Line "|---|---|"

    foreach ($Layer in $BackendLayers) {
        $RelativePath = $Layer.FullName.Replace($RepoRoot, "").TrimStart("\")
        Add-Line "| $($Layer.Name) | $RelativePath |"
    }
}
else {
    Add-Line "No se encontró services/api-dotnet/src."
}

Add-CommandOutput `
    -Title "Archivos principales del backend" `
    -CommandText "Get-ChildItem services/api-dotnet/src -Directory" `
    -Command {
        Get-ChildItem "services/api-dotnet/src" -Directory | Select-Object Name, FullName | Format-Table -AutoSize
    } `
    -AllowFailure

Add-Section "2. Backend: build, pruebas y scanner de dependencias"

if ($SkipBuild) {
    Add-Line "Build y pruebas omitidas por parámetro -SkipBuild."
}
else {
    Push-Location (Join-Path $RepoRoot "services/api-dotnet")

    Add-CommandOutput `
        -Title "Restauración de dependencias .NET" `
        -CommandText "dotnet restore Caritas.Brigadas.sln" `
        -Command {
            dotnet restore "Caritas.Brigadas.sln"
        }

    Add-CommandOutput `
        -Title "Build Release con warnings como errores" `
        -CommandText "dotnet build Caritas.Brigadas.sln -c Release /p:TreatWarningsAsErrors=true" `
        -Command {
            dotnet build "Caritas.Brigadas.sln" -c Release /p:TreatWarningsAsErrors=true
        }

    Add-CommandOutput `
        -Title "Pruebas unitarias e integración" `
        -CommandText "dotnet test Caritas.Brigadas.sln -c Release --verbosity normal" `
        -Command {
            dotnet test "Caritas.Brigadas.sln" -c Release --verbosity normal
        }

    Add-CommandOutput `
        -Title "Scanner de dependencias vulnerables NuGet" `
        -CommandText "dotnet list Caritas.Brigadas.sln package --vulnerable --include-transitive" `
        -Command {
            dotnet list "Caritas.Brigadas.sln" package --vulnerable --include-transitive
        } `
        -AllowFailure

    Pop-Location
}

Add-Section "3. Verificadores y guardrails"

$GuardrailScripts = @(
    "scripts/validate-repo-governance-baseline.ps1",
    "scripts/validate-repository-security-baseline.ps1",
    "scripts/validate-supply-chain-baseline.ps1",
    "scripts/validate-testing-baseline.ps1",
    "scripts/verify-p5-10-patient-module-closure.ps1"
)

Add-Line "| Verificador | Existe |"
Add-Line "|---|---:|"

foreach ($Script in $GuardrailScripts) {
    $Exists = Test-Path (Join-Path $RepoRoot $Script)
    Add-Line "| $Script | $Exists |"
}

foreach ($Script in $GuardrailScripts) {
    if (Test-Path (Join-Path $RepoRoot $Script)) {
        Add-CommandOutput `
            -Title "Ejecución: $Script" `
            -CommandText "pwsh $Script" `
            -Command {
                pwsh $Script
            } `
            -AllowFailure
    }
}

Add-Section "4. Docker: empaquetado de la API"

if ($SkipDocker) {
    Add-Line "Docker omitido por parámetro -SkipDocker."
}
else {
    Add-CommandOutput `
        -Title "Docker build de la API" `
        -CommandText "docker build -f services/api-dotnet/src/Caritas.Brigadas.Api/Dockerfile -t caritas-brigadas-api:demo services/api-dotnet" `
        -Command {
            docker build -f "services/api-dotnet/src/Caritas.Brigadas.Api/Dockerfile" -t "caritas-brigadas-api:demo" "services/api-dotnet"
        } `
        -AllowFailure

    Add-CommandOutput `
        -Title "Imagen Docker generada" `
        -CommandText "docker images caritas-brigadas-api" `
        -Command {
            docker images "caritas-brigadas-api"
        } `
        -AllowFailure
}

Add-Section "5. GitHub Actions, Pull Requests y escaneos"

if ($SkipGitHubApi) {
    Add-Line "Consultas de GitHub omitidas por parámetro -SkipGitHubApi."
}
else {
    Add-CommandOutput `
        -Title "Workflows configurados" `
        -CommandText "gh workflow list" `
        -Command {
            gh workflow list
        } `
        -AllowFailure

    Add-CommandOutput `
        -Title "Últimas corridas de verify.yml" `
        -CommandText "gh run list --workflow verify.yml --limit 15" `
        -Command {
            gh run list --workflow "verify.yml" --limit 15
        } `
        -AllowFailure

    if (-not [string]::IsNullOrWhiteSpace($RunId)) {
        Add-CommandOutput `
            -Title "Detalle del workflow run indicado" `
            -CommandText "gh run view $RunId" `
            -Command {
                gh run view $RunId
            } `
            -AllowFailure
    }
    else {
        Add-Line ""
        Add-Line "Run ID no proporcionado. Para ver detalle de un run específico:"
        Add-Line ""
        Add-Line "    pwsh scripts/demo-final-evidencia-tecnica.ps1 -RunId RUN_ID"
    }

    Add-CommandOutput `
        -Title "Últimas corridas de dependency-review.yml" `
        -CommandText "gh run list --workflow dependency-review.yml --limit 10" `
        -Command {
            gh run list --workflow "dependency-review.yml" --limit 10
        } `
        -AllowFailure

    if (-not [string]::IsNullOrWhiteSpace($RunId)) {
        Add-CommandOutput `
            -Title "Descarga de SBOM generado por CI" `
            -CommandText "gh run download $RunId -n caritas-brigadas-api-sbom" `
            -Command {
                gh run download $RunId -n "caritas-brigadas-api-sbom"
            } `
            -AllowFailure
    }
    else {
        Add-Line ""
        Add-Line "SBOM no descargado porque no se proporcionó RunId."
        Add-Line ""
        Add-Line "Comando manual:"
        Add-Line ""
        Add-Line "    gh run download RUN_ID -n caritas-brigadas-api-sbom"
    }

    Add-CommandOutput `
        -Title "Pull Requests recientes" `
        -CommandText "gh pr list --state all --limit 30" `
        -Command {
            gh pr list --state all --limit 30
        } `
        -AllowFailure

    Add-CommandOutput `
        -Title "PRs mergeados recientes" `
        -CommandText "gh pr list --state merged --limit 50 --json number,title,mergedAt" `
        -Command {
            gh pr list --state merged --limit 50 --json number,title,mergedAt
        } `
        -AllowFailure

    if (-not [string]::IsNullOrWhiteSpace($PrNumber)) {
        Add-CommandOutput `
            -Title "Detalle del PR indicado" `
            -CommandText "gh pr view $PrNumber" `
            -Command {
                gh pr view $PrNumber
            } `
            -AllowFailure
    }
    else {
        Add-Line ""
        Add-Line "PR number no proporcionado. Para ver detalle de un PR específico:"
        Add-Line ""
        Add-Line "    pwsh scripts/demo-final-evidencia-tecnica.ps1 -PrNumber PR_NUMBER"
    }

    Add-CommandOutput `
        -Title "Protección de rama main" `
        -CommandText "gh api repos/CarlosSanchezGutierrez/caritas-brigadas-salud/branches/main/protection" `
        -Command {
            gh api "repos/CarlosSanchezGutierrez/caritas-brigadas-salud/branches/main/protection"
        } `
        -AllowFailure

    Add-CommandOutput `
        -Title "Alertas de code scanning" `
        -CommandText "gh api repos/CarlosSanchezGutierrez/caritas-brigadas-salud/code-scanning/alerts" `
        -Command {
            gh api "repos/CarlosSanchezGutierrez/caritas-brigadas-salud/code-scanning/alerts"
        } `
        -AllowFailure
}

Add-Section "6. Historial y trazabilidad de milestones"

Add-CommandOutput `
    -Title "Últimos commits" `
    -CommandText "git log --oneline -20" `
    -Command {
        git log --oneline -20
    } `
    -AllowFailure

Add-CommandOutput `
    -Title "Commits relacionados con P5" `
    -CommandText "git log --oneline --grep='P5'" `
    -Command {
        git log --oneline --grep="P5"
    } `
    -AllowFailure

Add-CommandOutput `
    -Title "Tags / snapshots / milestones" `
    -CommandText "git tag" `
    -Command {
        git tag
    } `
    -AllowFailure

Add-Section "7. Lectura técnica del esfuerzo"

Add-Line "El repositorio evidencia una construcción backend seria y progresiva:"
Add-Line ""
Add-Line "- Arquitectura por capas en .NET."
Add-Line "- Separación de API, contratos, dominio, infraestructura y persistencia."
Add-Line "- SQL Server como fuente operacional de verdad."
Add-Line "- Validaciones por organización y protección contra cruces de datos."
Add-Line "- Auditoría de escrituras críticas."
Add-Line "- Trazabilidad por endpoints, entidades y operaciones."
Add-Line "- Idempotencia para operaciones sensibles."
Add-Line "- Metadata offline-first para futura sincronización móvil."
Add-Line "- Health checks, OpenAPI/Swagger y guardrails técnicos."
Add-Line "- Documentación operativa, QA, implementación y runbooks."
Add-Line "- Verificadores PowerShell para cierre de milestones."
Add-Line "- GitHub Actions como compuerta de calidad."
Add-Line "- Pull Requests como historial de avance y revisión."
Add-Line "- Build con warnings tratados como errores."
Add-Line "- Scanner de dependencias vulnerables."
Add-Line "- Preparación para supply chain evidence como SBOM."
Add-Line ""
Add-Line "Este documento no afirma producción real. La conexión al servidor SQL del Tec y la configuración del servidor institucional de Cáritas siguen pendientes como infraestructura real."

Add-Section "8. Resumen para demo"

Add-Line "| Área | Evidencia demostrable |"
Add-Line "|---|---|"
Add-Line "| Magnitud del repo | Conteo de archivos, docs, scripts, pruebas y capas backend |"
Add-Line "| Backend | Restore, build, test, scanner de dependencias |"
Add-Line "| Seguridad | Validadores de seguridad, dependency review, branch protection, no secrets |"
Add-Line "| Auditoría | Action mappers, audit logs, trazabilidad de operaciones críticas |"
Add-Line "| Gobernanza | Pull Requests, CI gates, docs, runbooks, verifiers |"
Add-Line "| Deploy técnico | Docker build de la API |"
Add-Line "| Supply chain | SBOM, dependency scanning, workflows |"
Add-Line "| Milestones | Historial Git, PRs mergeados, verificadores P5 |"
Add-Line "| Pendiente real | Servidor SQL Tec, servidor Cáritas, monitoreo, backups, aprobación institucional |"

Add-Line ""
Add-Line "Conclusión:"
Add-Line ""
Add-Line "Cáritas Brigadas de Salud debe presentarse como una plataforma institucional en evolución, no como una app aislada. El proyecto ya muestra esfuerzo real de ingeniería backend, seguridad, trazabilidad, auditoría, documentación, pruebas y gobierno técnico. La producción real queda condicionada a evidencia institucional, configuración de infraestructura real, servidor SQL, monitoreo, respaldo, seguridad operativa, revisión legal y piloto controlado."

$ReportContent = $ReportLines -join [Environment]::NewLine

Write-Utf8NoBom -Path $ReportPath -Content $ReportContent

Write-Host ""
Write-Host "Documento generado:"
Write-Host $ReportPath