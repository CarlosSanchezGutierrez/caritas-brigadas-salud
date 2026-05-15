# P3 Structured Logging and Correlation ID Baseline

Status: active
Scope: structured request logging and correlation id hardening
Target phase: P3-26F
Depends on: P3-26E health endpoint and deployment smoke implementation

---

## 1. Purpose

P3-26F hardens request telemetry and correlation id behavior.

The goal is to make production request diagnostics useful without exposing sensitive payloads or raw clinical resource identifiers.

---

## 2. Correlation id requirements

The API must support X-Correlation-Id.

The correlation id middleware must:

- read X-Correlation-Id from the request;
- validate the incoming value;
- reject empty values;
- reject values longer than MaxCorrelationIdLength;
- allow only safe ASCII characters;
- fall back to HttpContext.TraceIdentifier when the incoming value is unsafe;
- store the final value in HttpContext.Items;
- echo the final value in the response header.

---

## 3. Request telemetry requirements

Request telemetry must capture:

- CorrelationId;
- RequestId;
- HttpMethod;
- EndpointRoute;
- StatusCode;
- ElapsedMilliseconds.

Request telemetry must use ILogger.BeginScope so every log event has structured diagnostic context.

---

## 4. Sanitization requirements

Request telemetry must never log raw sensitive endpoint paths for:

- patients;
- patient-visits;
- service-encounters;
- form-responses;
- consent-documents;
- sync-batches.

Sensitive endpoint paths must be represented as:

/api/v1/[sensitive-resource]

Request telemetry must not log:

- raw PayloadJson;
- request bodies;
- patient names;
- phone numbers;
- national identifiers;
- bearer tokens;
- connection strings.

---

## 5. Logging level requirements

Request telemetry must log:

- Information for successful responses below 400;
- Warning for responses from 400 to 499;
- Error for responses 500 or greater;
- Error with exception for unhandled exceptions.

---

## 6. Production observability impact

P3-26F implements part of the P3-26D observability baseline.

Production go-live remains blocked until structured logging is validated with real deployment evidence.

---

## 7. Non-goals

P3-26F does not implement OpenTelemetry.

P3-26F does not implement cloud log export.

P3-26F does not create alert rules.

P3-26F does not approve production go-live.

---

## 8. Acceptance criteria

P3-26F is complete when:

- CorrelationIdMiddleware validates incoming correlation ids;
- RequestTelemetryMiddleware uses context.GetCorrelationId;
- RequestTelemetryMiddleware uses sanitized paths;
- RequestTelemetryMiddleware adds structured scope fields;
- RequestTelemetryMiddleware logs by response severity;
- contract tests validate the logging and correlation baseline;
- repository governance validation includes the structured logging verifier;
- dotnet build and dotnet test pass.