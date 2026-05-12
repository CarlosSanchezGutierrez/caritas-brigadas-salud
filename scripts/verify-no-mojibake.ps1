$ErrorActionPreference = "Stop"

$RepoRoot = Split-Path -Parent $PSScriptRoot

# ASCII-safe mojibake detection.
# Avoid literal mojibake strings in this script so the script itself cannot break parsing.
$SuspiciousChars = @(
    [char]0x00C3, # A with tilde: common mojibake marker
    [char]0x00C2, # A with circumflex: common mojibake marker
    [char]0x00A2  # cent sign: common second-order mojibake marker
)

$SuspiciousAsciiFragments = @(
    "organizaci",
    "configuraci",
    "auditor",
    "Administraci",
    "Coordinaci",
    "atenci",
    "psicolog",
    "odontolog",
    "revisi",
    "metricas"
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
    $relativePath = Resolve-Path $file.FullName -Relative

    foreach ($suspiciousChar in $SuspiciousChars) {
        if ($content.Contains([string]$suspiciousChar)) {
            $matches.Add("$relativePath contains suspicious mojibake char U+$('{0:X4}' -f [int][char]$suspiciousChar)")
        }
    }

    foreach ($fragment in $SuspiciousAsciiFragments) {
        $index = $content.IndexOf($fragment, [StringComparison]::OrdinalIgnoreCase)

        if ($index -ge 0) {
            $windowStart = [Math]::Max(0, $index - 10)
            $windowLength = [Math]::Min(80, $content.Length - $windowStart)
            $window = $content.Substring($windowStart, $windowLength)

            foreach ($suspiciousChar in $SuspiciousChars) {
                if ($window.Contains([string]$suspiciousChar)) {
                    $matches.Add("$relativePath contains mojibake near fragment '$fragment'")
                }
            }
        }
    }
}

if ($matches.Count -gt 0) {
    $matches | Sort-Object -Unique | ForEach-Object { Write-Error $_ }
    throw "Mojibake/UTF-8 corruption detected."
}

Write-Host "No mojibake patterns detected."