# P3.5-08 Mobile and API Offline Readiness Contract

## Current decision

Status: BLOCKED

Mobile and API offline readiness is not approved until real evidence exists for API versioning, offline sync behavior, retry safety, idempotency, conflict handling, local storage protection, mobile release configuration, App Store/Play Store readiness, and web admin API dependency.

## Scope

This contract applies to:

- ASP.NET Core API.
- iOS app.
- Android app.
- Web admin.
- Offline sync.
- SQL Server access boundaries.
- OpenAPI contracts.
- Authentication.
- Authorization.
- Local mobile storage.
- App Store release.
- Play Store release.
- Admin reporting.

## Non-negotiable rule

Clients must never connect directly to SQL Server.

Approved architecture:

```text
iOS / Android / Web Admin
        |
        | HTTPS only
        v
ASP.NET Core API
        |
        | controlled server-side SQL connection
        v
SQL Server

Forbidden:

SQL credentials in iOS.
SQL credentials in Android.
SQL credentials in web frontend bundles.
Backend secrets in clients.
Direct client-to-SQL Server writes.
Offline clients bypassing API sync.
Mobile-first goal

The system must support doctors and collaborators using iPhone and Android devices.

iPads may be supported, but must not be the only viable production client.

API readiness evidence
Evidence itemRequiredCurrent status
API base URL per environmentYesPENDING
API versioning strategyYesPENDING
OpenAPI contractYesPENDING
Error response contractYesPENDING
Authentication required for protected endpointsYesPENDING
Server-side authorizationYesPENDING
Organization boundary enforcementYesPENDING
Rate limitingYesPENDING
Request body limitsYesPENDING
Sync payload limitsYesPENDING
CORS and AllowedHostsYesPENDING
Health/readiness endpointsYesPENDING
Deployment smoke endpointYesPENDING
Backward compatibility policyYesPENDING
Offline sync evidence
Evidence itemRequiredCurrent status
Local event idYesPENDING
Client instance idYesPENDING
Device id decisionYesPENDING
Idempotency key behaviorYesPENDING
Retry behaviorYesPENDING
Duplicate event behaviorYesPENDING
Cross-batch duplicate behaviorYesPENDING
Pending event behaviorYesPENDING
Accepted event behaviorYesPENDING
Rejected event behaviorYesPENDING
Conflict event behaviorYesPENDING
Failed batch behaviorYesPENDING
Ordering behaviorYesPENDING
Partial batch behaviorYesPENDING
Corrupted payload behaviorYesPENDING
Audit traceabilityYesPENDING
Conflict handling evidence

Clients must handle:

Conflict or failureRequired client behaviorCurrent status
patient_id_already_existsShow recoverable conflictPENDING
patient_folio_already_existsShow recoverable conflictPENDING
patient_folio_duplicate_in_pending_batchShow recoverable conflictPENDING
invalid payloadShow rejection details safelyPENDING
unsupported entity typeShow sync incompatibilityPENDING
unsupported operationShow sync incompatibilityPENDING
authorization failureRequire re-auth or permissionsPENDING
expired sessionRequire login/session refreshPENDING
server unavailableQueue/retry safelyPENDING
readiness failureRetry later, do not drop dataPENDING
rate limit responseBackoff and retryPENDING
cross-batch duplicate eventTreat as conflictPENDING
sync batch already processedTreat as idempotent resultPENDING
Retry and idempotency evidence

Required behavior:

RequirementRequiredCurrent status
Retrying same batch does not create empty retry batchesYesPENDING
Retrying same event does not duplicate domain recordsYesPENDING
Cross-batch duplicate idempotency returns conflictYesPENDING
Device idempotency key fits storage limitYesPENDING
Non-device idempotency key fits storage limitYesPENDING
Retry response is deterministicYesPENDING
Offline replay is testableYesPENDING
Failed replay remains inspectableYesPENDING
Mobile local storage evidence
Evidence itemRequiredCurrent status
Local storage engineYesPENDING
Local encryption strategyYesPENDING
Keychain strategy for iOSYesPENDING
Keystore strategy for AndroidYesPENDING
Token storageYesPENDING
Offline queue retentionYesPENDING
Local wipeYesPENDING
Lost-device responseYesPENDING
Remote revocationYesPENDING
Session timeoutYesPENDING
Logout behaviorYesPENDING
Background snapshot/cache behaviorYesPENDING
App upgrade migration behaviorYesPENDING
Local schema migration behaviorYesPENDING
Mobile release configuration evidence
Evidence itemRequiredCurrent status
iOS environment-specific API base URLYesPENDING
Android environment-specific API base URLYesPENDING
OIDC public client configurationYesPENDING
Redirect URIYesPENDING
No SQL credentials in app bundleYesPENDING
No backend secrets in app bundleYesPENDING
No client secrets in app bundleYesPENDING
No admin tokens in app bundleYesPENDING
TestFlight configurationYesPENDING
Android internal testing configurationYesPENDING
App Store production configurationYesPENDING
Play Store production configurationYesPENDING
Minimum supported app version decisionYesPENDING
Forced update decisionDecision requiredPENDING
API compatibility evidence
Evidence itemRequiredCurrent status
Breaking change policyYesPENDING
Deprecation policyYesPENDING
Minimum supported client versionYesPENDING
Mobile compatibility windowYesPENDING
Error contract compatibilityYesPENDING
Sync payload compatibilityYesPENDING
OpenAPI contract freeze processYesPENDING
Release notesYesPENDING
Rollback compatibilityYesPENDING
Feature flag compatibilityYesPENDING
Web admin API readiness evidence
Evidence itemRequiredCurrent status
Web admin uses API onlyYesPENDING
No direct SQL Server accessYesPENDING
HTTPS-only API accessYesPENDING
Authenticated admin endpointsYesPENDING
Server-side authorizationYesPENDING
Export permissionsYesPENDING
Report permissionsYesPENDING
Export audit loggingYesPENDING
No secrets in frontend bundleYesPENDING
API error handlingYesPENDING
API version compatibilityYesPENDING
App Store and Play Store readiness evidence
Evidence itemRequiredCurrent status
Privacy policyYesPENDING
Data collection disclosureYesPENDING
Permissions justificationYesPENDING
Offline storage disclosureYesPENDING
Account access removal pathYesPENDING
Support contactYesPENDING
Incident response pathYesPENDING
Release signing controlledYesPENDING
Build provenance documentedYesPENDING
Production config reviewedYesPENDING
No secrets embeddedYesPENDING
Security release gate passedYesPENDING
Current readiness
StateValue
API readinessBLOCKED
Offline sync readinessBLOCKED
Retry/idempotency readinessBLOCKED
Mobile local storage readinessBLOCKED
iOS release readinessBLOCKED
Android release readinessBLOCKED
Web admin API readinessBLOCKED
App Store readinessBLOCKED
Play Store readinessBLOCKED
Production mobile/API readinessBLOCKED
Next required evidence
Define API versioning strategy.
Define mobile environment configuration.
Define iOS local storage strategy.
Define Android local storage strategy.
Define offline queue retention.
Define client conflict UX behavior.
Define minimum supported app version.
Define App Store privacy requirements.
Define Play Store privacy requirements.
Verify no client connects directly to SQL Server.