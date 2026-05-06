$ErrorActionPreference = "Stop"

$RepoRoot = Split-Path -Parent $PSScriptRoot
$BackendRoot = Join-Path $RepoRoot "services\api-dotnet"
$WebAppRoot = Join-Path $RepoRoot "apps\web-next"
$ReportsRoot = Join-Path $RepoRoot "security-reports"
$Timestamp = Get-Date -Format "yyyyMMdd-HHmmss"
$ReportPath = Join-Path $ReportsRoot "security-audit-$Timestamp.md"

function Add-ReportLine {
    param([string]$Line)
    Add-Content -Path $ReportPath -Value $Line -Encoding UTF8
}

function Add-CommandSection {
    param(
        [string]$Title,
        [scriptblock]$Command
    )

    Add-ReportLine ""
    Add-ReportLine "## $Title"
    Add-ReportLine ""
    Add-ReportLine "``````text"

    try {
        $Output = & $Command 2>&1 | Out-String
        Add-Content -Path $ReportPath -Value $Output -Encoding UTF8
    }
    catch {
        Add-Content -Path $ReportPath -Value $_.Exception.Message -Encoding UTF8
    }

    Add-ReportLine "``````"
}

Set-Location $RepoRoot
New-Item -ItemType Directory -Path $ReportsRoot -Force | Out-Null

Set-Content -Path $ReportPath -Encoding UTF8 -Value "# Security Audit Report"
Add-ReportLine ""
Add-ReportLine "Proyecto: Cáritas Brigadas de Salud"
Add-ReportLine "Fecha: $(Get-Date -Format 'yyyy-MM-dd HH:mm:ss')"
Add-ReportLine ""

$SolutionPath = @(Get-ChildItem -Path $BackendRoot -File -Filter "*.sln")[0].FullName

Add-CommandSection "Git status" {
    Set-Location $RepoRoot
    git status
}

Add-CommandSection "Backend build with warnings as errors" {
    Set-Location $BackendRoot
    dotnet build $SolutionPath /p:TreatWarningsAsErrors=true
}

Add-CommandSection "Backend tests with warnings as errors" {
    Set-Location $BackendRoot
    dotnet test $SolutionPath /p:TreatWarningsAsErrors=true
}

Add-CommandSection "NuGet vulnerable packages" {
    Set-Location $BackendRoot
    dotnet list $SolutionPath package --vulnerable --include-transitive
}

Add-CommandSection "NuGet outdated packages" {
    Set-Location $BackendRoot
    dotnet list $SolutionPath package --outdated
}

Add-CommandSection "Frontend npm audit moderate" {
    Set-Location $WebAppRoot
    npm audit --audit-level=moderate
}

Add-CommandSection "Search: possible secrets" {
    Set-Location $RepoRoot
    Select-String -Path ".\**\*" -Pattern "password\s*=|pwd\s*=|secret|api[_-]?key|connectionstring|token|private[_-]?key|BEGIN RSA PRIVATE KEY|BEGIN PRIVATE KEY" -CaseSensitive:$false -ErrorAction SilentlyContinue |
        Where-Object {
            $_.Path -notmatch "\\bin\\|\\obj\\|\\node_modules\\|\\.next\\|\\package-lock\.json|security-reports" -and
            $_.Line -notmatch "DefaultConnectionString|ConnectionStrings__SqlServer|localdb|TrustServerCertificate"
        } |
        Select-Object Path, LineNumber, Line |
        Format-List
}

Add-CommandSection "Search: dangerous CORS patterns" {
    Set-Location $RepoRoot
    Select-String -Path ".\services\api-dotnet\**\*.cs" -Pattern "AllowAnyOrigin|SetIsOriginAllowed|WithOrigins" -CaseSensitive:$false -ErrorAction SilentlyContinue |
        Select-Object Path, LineNumber, Line |
        Format-List
}

Add-CommandSection "Search: SQL injection risk patterns" {
    Set-Location $RepoRoot
    Select-String -Path ".\services\api-dotnet\**\*.cs" -Pattern "FromSqlRaw|ExecuteSqlRaw|SqlCommand|CommandText|string\.Format\(.*SELECT|SELECT .* \+" -CaseSensitive:$false -ErrorAction SilentlyContinue |
        Select-Object Path, LineNumber, Line |
        Format-List
}

Add-CommandSection "Search: frontend localStorage/sessionStorage" {
    Set-Location $RepoRoot
    Select-String -Path ".\apps\web-next\src\**\*.ts",".\apps\web-next\src\**\*.tsx" -Pattern "localStorage|sessionStorage|document\.cookie" -CaseSensitive:$false -ErrorAction SilentlyContinue |
        Select-Object Path, LineNumber, Line |
        Format-List
}

Add-CommandSection "Search: frontend console/debugger" {
    Set-Location $RepoRoot
    Select-String -Path ".\apps\web-next\src\**\*.ts",".\apps\web-next\src\**\*.tsx" -Pattern "console\.log|console\.debug|debugger" -CaseSensitive:$false -ErrorAction SilentlyContinue |
        Select-Object Path, LineNumber, Line |
        Format-List
}

Add-CommandSection "Search: backend sensitive logging candidates" {
    Set-Location $RepoRoot
    Select-String -Path ".\services\api-dotnet\**\*.cs" -Pattern "LogInformation|LogWarning|LogError|Console\.WriteLine" -CaseSensitive:$false -ErrorAction SilentlyContinue |
        Where-Object {
            $_.Line -match "patient|curp|phone|telefono|address|domicilio|signature|firma|diagnosis|diagnostico|token|password|secret|payload|body|json"
        } |
        Select-Object Path, LineNumber, Line |
        Format-List
}

Add-CommandSection "Search: TODO/HACK/TEMP/FIXME" {
    Set-Location $RepoRoot
    Select-String -Path ".\services\api-dotnet\**\*.cs",".\apps\web-next\src\**\*.ts",".\apps\web-next\src\**\*.tsx",".\**\*.md" -Pattern "\bTODO\b|\bFIXME\b|\bHACK\b|\bTEMP\b|\btemporary\b|\bworkaround\b|\bpendiente técnico\b|\bprovisional\b" -CaseSensitive:$false -ErrorAction SilentlyContinue |
        Where-Object {
            $_.Path -notmatch "\\bin\\|\\obj\\|\\node_modules\\|\\.next\\|security-reports"
        } |
        Select-Object Path, LineNumber, Line |
        Format-List
}

Add-CommandSection "Search: ASP.NET security middleware candidates" {
    Set-Location $RepoRoot
    Select-String -Path ".\services\api-dotnet\src\**\*.cs" -Pattern "UseHttpsRedirection|UseHsts|UseCors|UseAuthentication|UseAuthorization|UseRateLimiter|UseExceptionHandler|UseSwagger|MapHealthChecks|Use\(async" -CaseSensitive:$false -ErrorAction SilentlyContinue |
        Select-Object Path, LineNumber, Line |
        Format-List
}

Write-Host "Security audit report generated:" -ForegroundColor Green
Write-Host $ReportPath -ForegroundColor Green
Set-Location $RepoRoot