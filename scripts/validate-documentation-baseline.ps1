$ErrorActionPreference = "Stop"

$RepoRoot = Split-Path -Parent $PSScriptRoot
$ReadmePath = Join-Path $RepoRoot "README.md"
$StartHerePath = Join-Path $RepoRoot "docs\START_HERE.md"
$SystemOverviewPath = Join-Path $RepoRoot "docs\architecture\system-overview.md"
$FolderMapPath = Join-Path $RepoRoot "docs\architecture\folder-map.md"
$LocalDevPath = Join-Path $RepoRoot "docs\contributing\local-development.md"
$ContributionPathsPath = Join-Path $RepoRoot "docs\contributing\contribution-paths.md"
$MaintainerPlaybookPath = Join-Path $RepoRoot "docs\governance\maintainer-playbook.md"
$TiHandoffPath = Join-Path $RepoRoot "docs\operations\ti-handoff.md"
$SecurityMapPath = Join-Path $RepoRoot "docs\security\security-map.md"

function Assert-FileExists {
    param([string]$Path)

    if (-not (Test-Path $Path)) {
        throw "Required file not found: $Path"
    }
}

function Assert-Contains {
    param(
        [string]$Path,
        [string]$Token
    )

    $Content = Get-Content $Path -Raw

    if ($Content.IndexOf($Token, [StringComparison]::OrdinalIgnoreCase) -lt 0) {
        throw "$Path does not contain required token: $Token"
    }
}

$Files = @(
    $ReadmePath,
    $StartHerePath,
    $SystemOverviewPath,
    $FolderMapPath,
    $LocalDevPath,
    $ContributionPathsPath,
    $MaintainerPlaybookPath,
    $TiHandoffPath,
    $SecurityMapPath
)

foreach ($File in $Files) {
    Assert-FileExists $File
}

Assert-Contains $ReadmePath "Cáritas Brigadas de Salud"
Assert-Contains $ReadmePath "START_HERE"
Assert-Contains $StartHerePath "Las cinco capas"
Assert-Contains $SystemOverviewPath "ASP.NET Core API"
Assert-Contains $SystemOverviewPath "SQL Server"
Assert-Contains $FolderMapPath "services/api-dotnet"
Assert-Contains $FolderMapPath "apps/web-next"
Assert-Contains $LocalDevPath "verify-local"
Assert-Contains $ContributionPathsPath "Perfil: Frontend"
Assert-Contains $MaintainerPlaybookPath "Checks obligatorios"
Assert-Contains $TiHandoffPath "Antes de producción"
Assert-Contains $SecurityMapPath "CodeQL Default Setup"

Write-Host ""
Write-Host "============================================================" -ForegroundColor Green
Write-Host "DOCUMENTATION BASELINE VALIDATION PASO CORRECTAMENTE" -ForegroundColor Green
Write-Host "============================================================" -ForegroundColor Green
