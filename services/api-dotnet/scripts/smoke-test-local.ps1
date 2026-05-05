param(
    [string] $BaseUrl = "https://localhost:7044",
    [string] $OrganizationId = "",
    [string] $UserId = ""
)

$ErrorActionPreference = "Stop"

function Write-Step {
    param([string] $Message)

    Write-Host ""
    Write-Host "=== $Message ===" -ForegroundColor Cyan
}

function Invoke-Api {
    param(
        [Parameter(Mandatory = $true)]
        [string] $Method,

        [Parameter(Mandatory = $true)]
        [string] $Path,

        [object] $Body = $null,

        [int[]] $ExpectedStatus = @(200)
    )

    $url = "$BaseUrl$Path"
    $tempFile = $null

    $args = @(
        "-k",
        "-sS",
        "-w",
        "`n%{http_code}",
        "-X",
        $Method,
        $url
    )

    if ($null -ne $Body) {
        $tempFile = Join-Path ([System.IO.Path]::GetTempPath()) "caritas-smoke-$([Guid]::NewGuid()).json"
        $Body | ConvertTo-Json -Depth 30 | Set-Content -Path $tempFile -Encoding UTF8

        $args += @(
            "-H",
            "Content-Type: application/json",
            "--data-binary",
            "@$tempFile"
        )
    }

    $effectiveUserId = [string]$script:UserId
    $effectiveOrganizationId = [string]$script:OrganizationId

    if (-not [string]::IsNullOrWhiteSpace($effectiveUserId) -and
        -not [string]::IsNullOrWhiteSpace($effectiveOrganizationId)) {
        $args += @(
            "-H",
            "X-Dev-User-Id: $effectiveUserId",
            "-H",
            "X-Dev-Organization-Id: $effectiveOrganizationId",
            "-H",
            "X-Dev-Roles: SUPER_ADMIN",
            "-H",
            "X-Dev-Name: Smoke Test User",
            "-H",
            "X-Dev-Email: smoke.test@caritas.local"
        )
    }

    try {
        $raw = & curl.exe @args

        if ($LASTEXITCODE -ne 0) {
            throw "curl failed with exit code $LASTEXITCODE for $Method $url"
        }

        $lines = @($raw)
        $statusCode = [int]$lines[-1]
        $bodyText = ($lines[0..($lines.Count - 2)] -join "`n").Trim()

        if ($ExpectedStatus -notcontains $statusCode) {
            throw "Unexpected HTTP $statusCode for $Method $url. Body: $bodyText"
        }

        $json = $null

        if (-not [string]::IsNullOrWhiteSpace($bodyText)) {
            $json = $bodyText | ConvertFrom-Json
        }

        return [pscustomobject]@{
            StatusCode = $statusCode
            Body = $json
            Raw = $bodyText
        }
    }
    finally {
        if ($tempFile -and (Test-Path $tempFile)) {
            Remove-Item $tempFile -Force -ErrorAction SilentlyContinue
        }
    }
}

function Assert-Success {
    param(
        [Parameter(Mandatory = $true)]
        $Result,

        [Parameter(Mandatory = $true)]
        [string] $Name
    )

    if ($null -eq $Result.Body -or $Result.Body.success -ne $true) {
        throw "$Name failed. Raw: $($Result.Raw)"
    }

    Write-Host "OK: $Name" -ForegroundColor Green
}

function First-Item {
    param(
        [Parameter(Mandatory = $true)]
        $Data,

        [Parameter(Mandatory = $true)]
        [string] $Name
    )

    $items = @($Data)

    if ($items.Count -eq 0) {
        throw "No records found for $Name."
    }

    return $items[0]
}

$runId = Get-Date -Format "yyyyMMddHHmmss"

Write-Host "BaseUrl: $BaseUrl"
Write-Host "RunId:   $runId"

Write-Step "Health"
$health = Invoke-Api -Method GET -Path "/api/v1/health"
Assert-Success $health "health"

Write-Step "Organizations"
$organizations = Invoke-Api -Method GET -Path "/api/v1/organizations"
Assert-Success $organizations "list organizations"

if ([string]::IsNullOrWhiteSpace($OrganizationId)) {
    $organization = First-Item $organizations.Body.data "organizations"
    $OrganizationId = $organization.id
}

Write-Host "OrganizationId: $OrganizationId"

Write-Step "Users"
$users = Invoke-Api -Method GET -Path "/api/v1/organizations/$OrganizationId/users"
Assert-Success $users "list users"

if ([string]::IsNullOrWhiteSpace($UserId)) {
    if (@($users.Body.data).Count -eq 0) {
        $createUser = Invoke-Api `
            -Method POST `
            -Path "/api/v1/organizations/$OrganizationId/users" `
            -ExpectedStatus @(201, 200) `
            -Body @{
                fullName = "Smoke Test User $runId"
                email = "smoke.$runId@caritas.local"
                phone = "8100000000"
                username = "smoke.$runId"
            }

        Assert-Success $createUser "create user"
        $UserId = $createUser.Body.data.id
    }
    else {
        $UserId = @($users.Body.data)[0].id
    }
}

Write-Host "UserId: $UserId"

Write-Step "Services"
$seedServices = Invoke-Api -Method POST -Path "/api/v1/organizations/$OrganizationId/services/seed-defaults"
Assert-Success $seedServices "seed services"

$services = Invoke-Api -Method GET -Path "/api/v1/organizations/$OrganizationId/services"
Assert-Success $services "list services"

Write-Step "Form templates"
$seedTemplates = Invoke-Api -Method POST -Path "/api/v1/organizations/$OrganizationId/form-templates/seed-defaults"
Assert-Success $seedTemplates "seed form templates"

$formTemplates = Invoke-Api -Method GET -Path "/api/v1/organizations/$OrganizationId/form-templates"
Assert-Success $formTemplates "list form templates"

$formTemplate = @($formTemplates.Body.data | Where-Object { $_.formCode -eq "GENERAL_MEDICINE_V1" } | Select-Object -First 1)

if ($null -eq $formTemplate) {
    throw "GENERAL_MEDICINE_V1 form template was not found."
}

Write-Step "Communities"
$communities = Invoke-Api -Method GET -Path "/api/v1/organizations/$OrganizationId/communities"
Assert-Success $communities "list communities"

if (@($communities.Body.data).Count -eq 0) {
    $createCommunity = Invoke-Api `
        -Method POST `
        -Path "/api/v1/organizations/$OrganizationId/communities" `
        -ExpectedStatus @(201) `
        -Body @{
            state = "Nuevo León"
            municipality = "Monterrey"
            colony = "Smoke Test"
            communityName = "Comunidad Smoke $runId"
            addressReference = "Registro local de smoke test"
            riskLevel = "normal"
        }

    Assert-Success $createCommunity "create community"
}

Write-Step "Mobile units"
$mobileUnits = Invoke-Api -Method GET -Path "/api/v1/organizations/$OrganizationId/mobile-units"
Assert-Success $mobileUnits "list mobile units"

if (@($mobileUnits.Body.data).Count -eq 0) {
    $createMobileUnit = Invoke-Api `
        -Method POST `
        -Path "/api/v1/organizations/$OrganizationId/mobile-units" `
        -ExpectedStatus @(201) `
        -Body @{
            name = "Unidad Smoke $runId"
            unitType = "Unidad médica"
            plateNumber = $null
            description = "Unidad local de smoke test"
        }

    Assert-Success $createMobileUnit "create mobile unit"
}

Write-Step "Brigades"
$brigades = Invoke-Api -Method GET -Path "/api/v1/organizations/$OrganizationId/brigades"
Assert-Success $brigades "list brigades"

if (@($brigades.Body.data).Count -eq 0) {
    $createBrigade = Invoke-Api `
        -Method POST `
        -Path "/api/v1/organizations/$OrganizationId/brigades" `
        -ExpectedStatus @(201) `
        -Body @{
            name = "Brigada Smoke $runId"
            brigadeType = "Salud"
            scheduledDate = "2026-05-15"
            communityId = $null
            municipality = "Monterrey"
            colony = "Smoke Test"
            locationText = "Ubicación local de smoke test"
            mobileUnitId = $null
            coordinatorUserId = $UserId
        }

    Assert-Success $createBrigade "create brigade"
    $brigadeId = $createBrigade.Body.data.id
}
else {
    $brigadeId = @($brigades.Body.data)[0].id
}

Write-Host "BrigadeId: $brigadeId"

Write-Step "Brigade services"
$assignService = Invoke-Api `
    -Method POST `
    -Path "/api/v1/brigades/$brigadeId/services" `
    -ExpectedStatus @(201, 409) `
    -Body @{
        serviceCode = "GENERAL_MEDICINE"
        capacityEstimate = 50
        assignedLeadUserId = $null
    }

if ($assignService.StatusCode -eq 409) {
    Write-Host "OK: GENERAL_MEDICINE already assigned to brigade" -ForegroundColor Yellow
}
else {
    Assert-Success $assignService "assign general medicine"
}

$brigadeServices = Invoke-Api -Method GET -Path "/api/v1/brigades/$brigadeId/services"
Assert-Success $brigadeServices "list brigade services"

Write-Step "Patients"
$createPatient = Invoke-Api `
    -Method POST `
    -Path "/api/v1/organizations/$OrganizationId/patients" `
    -ExpectedStatus @(201) `
    -Body @{
        patientFolio = $null
        firstName = "Paciente"
        paternalLastName = "Smoke"
        maternalLastName = "$runId"
        birthDate = "1990-05-10"
        approximateAge = $null
        sex = "masculino"
        curp = $null
        phone = "8111111111"
        addressLine = "Domicilio smoke test"
        municipality = "Monterrey"
        colony = "Smoke Test"
        community = "Comunidad Smoke"
        isMigrant = $false
        isPartialRecord = $false
        partialRecordReason = $null
        notesAdmin = "Paciente generado por smoke test local."
    }

Assert-Success $createPatient "create patient"
$patientId = $createPatient.Body.data.id

$patients = Invoke-Api -Method GET -Path "/api/v1/organizations/$OrganizationId/patients"
Assert-Success $patients "list patients"

Write-Step "Patient visits"
$createVisit = Invoke-Api `
    -Method POST `
    -Path "/api/v1/organizations/$OrganizationId/patient-visits" `
    -ExpectedStatus @(201) `
    -Body @{
        visitFolio = $null
        patientId = $patientId
        brigadeId = $brigadeId
        arrivalTime = $null
        registeredByUserId = $UserId
        createdOffline = $false
        deviceId = $null
    }

Assert-Success $createVisit "create patient visit"
$visitId = $createVisit.Body.data.id

$visits = Invoke-Api -Method GET -Path "/api/v1/organizations/$OrganizationId/patient-visits"
Assert-Success $visits "list patient visits"

Write-Step "Service encounters"
$createEncounter = Invoke-Api `
    -Method POST `
    -Path "/api/v1/organizations/$OrganizationId/service-encounters" `
    -ExpectedStatus @(201) `
    -Body @{
        encounterFolio = $null
        visitId = $visitId
        serviceCode = "GENERAL_MEDICINE"
        providerUserId = $UserId
        startedAt = $null
        createdOffline = $false
        deviceId = $null
    }

Assert-Success $createEncounter "create service encounter"
$encounterId = $createEncounter.Body.data.id

$encounters = Invoke-Api -Method GET -Path "/api/v1/organizations/$OrganizationId/service-encounters"
Assert-Success $encounters "list service encounters"

Write-Step "Form responses"
$responseJson = @{
    chiefComplaint = "Dolor de cabeza leve desde ayer"
    bloodPressure = "120/80"
    temperatureCelsius = 36.7
    weightKg = 72
    clinicalNotes = "Paciente estable. Registro generado por smoke test."
    recommendations = "Hidratación y observación."
    requiresFollowUp = $false
    requiresReferral = $false
} | ConvertTo-Json -Depth 10 -Compress

$createFormResponse = Invoke-Api `
    -Method POST `
    -Path "/api/v1/organizations/$OrganizationId/form-responses" `
    -ExpectedStatus @(201) `
    -Body @{
        encounterId = $encounterId
        formTemplateId = $formTemplate.id
        responseJson = $responseJson
        submittedByUserId = $UserId
        submittedAt = $null
        createdOffline = $false
        deviceId = $null
    }

Assert-Success $createFormResponse "create form response"

$formResponses = Invoke-Api -Method GET -Path "/api/v1/organizations/$OrganizationId/form-responses"
Assert-Success $formResponses "list form responses"

Write-Step "Consent documents"
$createConsent = Invoke-Api `
    -Method POST `
    -Path "/api/v1/organizations/$OrganizationId/consent-documents" `
    -ExpectedStatus @(201) `
    -Body @{
        patientId = $patientId
        visitId = $visitId
        consentType = "PRIVACY_NOTICE"
        documentVersion = "smoke-$runId"
        documentTextSnapshot = "Aviso de privacidad local generado por smoke test."
        signatureDataUrl = "data:image/png;base64,TEST_SIGNATURE_PLACEHOLDER"
        guardianFullName = $null
        guardianRelationship = $null
        signedByUserId = $UserId
        signedAt = $null
        createdOffline = $false
        deviceId = $null
    }

Assert-Success $createConsent "create consent document"

$consents = Invoke-Api -Method GET -Path "/api/v1/organizations/$OrganizationId/consent-documents"
Assert-Success $consents "list consent documents"

Write-Step "Reports"
$summary = Invoke-Api -Method GET -Path "/api/v1/organizations/$OrganizationId/reports/summary"
Assert-Success $summary "reports summary"

Write-Step "Sync batches"
$deviceId = [Guid]::NewGuid()

$payload = @{
    events = @(
        @{
            type = "offline-note"
            localId = "local-$runId"
            entity = "patient-visit"
            operation = "upsert"
            timestampUtc = (Get-Date).ToUniversalTime().ToString("o")
            data = @{
                note = "Lote offline generado por smoke test local."
            }
        }
    )
} | ConvertTo-Json -Depth 10 -Compress

$createSyncBatch = Invoke-Api `
    -Method POST `
    -Path "/api/v1/organizations/$OrganizationId/sync-batches" `
    -ExpectedStatus @(201) `
    -Body @{
        userId = $UserId
        brigadeId = $brigadeId
        deviceId = $deviceId
        payloadJson = $payload
        eventsCount = $null
        startedAt = $null
    }

Assert-Success $createSyncBatch "create sync batch"

$syncBatches = Invoke-Api -Method GET -Path "/api/v1/organizations/$OrganizationId/sync-batches"
Assert-Success $syncBatches "list sync batches"

Write-Step "Audit logs"
$auditLogs = Invoke-Api -Method GET -Path "/api/v1/organizations/$OrganizationId/audit-logs"
Assert-Success $auditLogs "list audit logs"

Write-Host ""
Write-Host "SMOKE TEST COMPLETED SUCCESSFULLY" -ForegroundColor Green
Write-Host "OrganizationId: $OrganizationId"
Write-Host "UserId:         $UserId"
Write-Host "BrigadeId:      $brigadeId"
Write-Host "PatientId:      $patientId"
Write-Host "VisitId:        $visitId"
Write-Host "EncounterId:    $encounterId"


