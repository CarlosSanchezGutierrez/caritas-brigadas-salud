# P3.5-08 Mobile and API Offline Readiness Contract Baseline

## Status

Required before iOS, Android, Web Admin, App Store, Play Store, staging pilot, or production release.

This document is not a production approval.

## Purpose

Define the required backend and client readiness rules for mobile-first usage, offline sync, API versioning, retry safety, idempotency, conflict handling, local storage, release configuration, App Store/Play Store deployment, and web admin API dependency.

## Core rule

Mobile and web clients must never connect directly to SQL Server.

Approved path:

- iOS -> HTTPS -> API -> SQL Server.
- Android -> HTTPS -> API -> SQL Server.
- Web Admin -> HTTPS -> API -> SQL Server.

Forbidden path:

- iOS -> SQL Server.
- Android -> SQL Server.
- Web Admin -> SQL Server.
- SQL credentials embedded in clients.
- Backend secrets embedded in clients.
- Offline clients writing directly to the database.

## Mobile-first production goal

The system must support collaborators and doctors using personal or organization-owned iPhone and Android devices without requiring iPads as the only supported client.

Required backend support:

- Stable API contract.
- HTTPS-only communication.
- Token-based authentication.
- API versioning.
- Offline queue.
- Idempotent sync.
- Retry-safe sync.
- Conflict-aware sync.
- Controlled payload limits.
- Safe error contracts.
- Client instance/device identity.
- Local storage protection.
- Environment-specific configuration.

## API readiness requirements

The API must define:

- API base URL per environment.
- API versioning strategy.
- OpenAPI contract.
- Error response contract.
- Authentication requirement.
- Authorization requirement.
- Organization boundary enforcement.
- Rate limiting.
- Request body limits.
- Sync payload limits.
- CORS and AllowedHosts.
- Health/readiness endpoints.
- Deployment smoke endpoint.
- Backward compatibility policy.

## Offline sync requirements

Offline sync must define:

- Local event id.
- Client instance id.
- Device id decision.
- Idempotency key behavior.
- Retry behavior.
- Duplicate event behavior.
- Cross-batch duplicate behavior.
- Pending event behavior.
- Accepted event behavior.
- Rejected event behavior.
- Conflict event behavior.
- Failed batch behavior.
- Ordering behavior.
- Partial batch behavior.
- Corrupted payload behavior.
- Manual recovery behavior.
- Audit traceability.

## Conflict handling requirements

Clients must be able to display and recover from:

- patient_id_already_exists.
- patient_folio_already_exists.
- patient_folio_duplicate_in_pending_batch.
- invalid payload.
- unsupported entity type.
- unsupported operation.
- authorization failure.
- expired session.
- server unavailable.
- readiness failure.
- rate limit response.
- cross-batch duplicate event.
- sync batch already processed.

## Retry and idempotency requirements

Required behavior:

- Retrying the same batch must not create empty retry batches.
- Retrying the same event must not duplicate domain records.
- Cross-batch duplicate idempotency keys must return a conflict response.
- Device-based idempotency keys must remain within storage limits.
- Non-device idempotency keys must remain within storage limits.
- Idempotency behavior must be testable.
- Offline clients must treat retry responses as deterministic.

## Mobile local storage requirements

iOS and Android must define:

- Local storage engine.
- Local encryption.
- Keychain/Keystore strategy.
- Token storage.
- Offline queue retention.
- Local wipe.
- Lost-device response.
- Remote revocation.
- Session timeout.
- Logout behavior.
- Background snapshot/cache behavior.
- App upgrade migration behavior.
- Local schema migration behavior.

## Mobile release configuration requirements

iOS and Android release builds must define:

- Environment-specific API base URL.
- OIDC/public client configuration.
- Redirect URI.
- No SQL credentials.
- No backend secrets.
- No client secrets.
- No admin tokens.
- No production connection strings.
- No migration credentials.
- App Store configuration.
- Play Store configuration.
- TestFlight/internal testing configuration.
- Android internal testing configuration.
- Minimum supported app version decision.
- Forced update decision.

## API compatibility requirements

The backend must define:

- Breaking change policy.
- Deprecation policy.
- Minimum supported client version.
- Mobile client compatibility window.
- Error contract compatibility.
- Sync payload compatibility.
- OpenAPI contract freeze process.
- Release notes.
- Rollback compatibility.
- Feature flag compatibility.

## Web admin API readiness requirements

The web admin must depend on the API, not direct SQL Server access.

Required:

- HTTPS-only API access.
- Authenticated admin endpoints.
- Server-side authorization.
- Export permissions.
- Report permissions.
- Audit logging for exports.
- No secrets in frontend bundle.
- API error handling.
- API version compatibility.

## App Store and Play Store readiness requirements

Before release:

- Privacy policy exists.
- Data collection disclosure is reviewed.
- Permissions are justified.
- Offline storage behavior is documented.
- Account deletion or access removal path is defined if required.
- Support contact is defined.
- Incident response path is defined.
- Release signing is controlled.
- Build provenance is documented.
- Production config is reviewed.
- No secrets are embedded.
- Security release gate is passed.

## Production readiness states

- BLOCKED.
- READY FOR STAGING MOBILE/API.
- READY FOR PILOT MOBILE/API.
- READY FOR APP STORE REVIEW.
- READY FOR PLAY STORE REVIEW.
- READY FOR PRODUCTION MOBILE/API.

Default state is BLOCKED.