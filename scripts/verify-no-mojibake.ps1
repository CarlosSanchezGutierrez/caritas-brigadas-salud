$ErrorActionPreference = "Stop"

$RepoRoot = Split-Path -Parent $PSScriptRoot

$Patterns = @(
    "Ã",
    "Â",
    "â€™",
    "â€œ",
    "â€",
    "â€“",
    "â€”",
    "organizaciÃ",
    "configuraciÃ",
    "auditorÃ",
    "AdministraciÃ",
    "CoordinaciÃ",
    "atenciÃ",
    "psicologÃ",
    "odontologÃ",
    "revisiÃ",
    "mÃ©tricas"
)

$TargetExtensions = @(
    ".cs",
    ".csproj",
    ".json",
    ".md",
    ".ps1",
    ".yml",
    ".yaml",
    ".sql",
    ".ts",
    ".tsx",
    ".js",
    ".jsx"
)

$ExcludedSegments = @(
    "\bin\",
    "\obj\",
    "\node_modules\",
    "\.git\",
    "\.next\",
    "\coverage\",
    "\playwright-report\",
    "\test-results\"
)

$files = Get-ChildItem $RepoRoot -Recurse -File |
    Where-Object {
        $path = $_.FullName
        $include = $TargetExtensions -contains $_.Extension

        foreach ($segment in $ExcludedSegments) {
            if ($path.Contains($segment)) {
                $include = $false
                break
            }
        }

        $include
    }

$matches = New-Object System.Collections.Generic.List[string]

foreach ($file in $files) {
    $content = Get-Content $file.FullName -Raw -Encoding UTF8

    foreach ($pattern in $Patterns) {
        if ($content.Contains($pattern)) {
            $relativePath = Resolve-Path $file.FullName -Relative
            $matches.Add("$relativePath contains mojibake pattern: $pattern")
        }
    }
}

if ($matches.Count -gt 0) {
    $matches | ForEach-Object { Write-Error $_ }
    throw "Mojibake/UTF-8 corruption detected."
}

Write-Host "No mojibake patterns detected."