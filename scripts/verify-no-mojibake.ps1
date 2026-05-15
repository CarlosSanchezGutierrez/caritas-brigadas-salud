$ErrorActionPreference = "Stop"

$RepoRoot = Split-Path -Parent $PSScriptRoot

$SuspiciousChars = @(
    [char]0x00C3,
    [char]0x00C2,
    [char]0x00A2
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
    "\test-results\",
    "\security-reports\"
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
    $relativePath = Resolve-Path $file.FullName -Relative

    foreach ($suspiciousChar in $SuspiciousChars) {
        if ($content.Contains([string]$suspiciousChar)) {
            $matches.Add("$relativePath contains suspicious mojibake char U+$('{0:X4}' -f [int][char]$suspiciousChar)")
        }
    }
}

if ($matches.Count -gt 0) {
    $matches | Sort-Object -Unique | ForEach-Object { Write-Error $_ }
    throw "Mojibake/UTF-8 corruption detected."
}

Write-Host "No mojibake patterns detected."
