# P3 Health Endpoint and Deployment Smoke Baseline

Status: active
Scope: health endpoint implementation and deployment smoke testing
Target phase: P3-26E
Depends on: P3-26D production observability baseline

---

## 1. Purpose

P3-26E implements production-oriented health endpoints and a deployment smoke script.

This is the first implementation step after the production observability baseline.

---

## 2. Health endpoints

The API must expose:

- GET /health/live;
- GET /health/ready.

The live endpoint verifies process availability.

The ready endpoint verifies dependency readiness, including database connectivity.

---

## 3. Health response requirements

Health responses must be JSON and include:

- status;
- timestampUtc;
- correlationId;
- totalDurationMilliseconds;
- checks;
- check name;
- check status;
- check description;
- check durationMilliseconds;
- check tags.

Health responses must not expose:

- connection strings;
- passwords;
- bearer tokens;
- raw PayloadJson;
- patient names;
- phone numbers;
- SQL Server connection details;
- stack traces.

---

## 4. Health registration requirements

Program.cs must register:

- api-live health check tagged as live;
- database health check tagged as ready;
- /health/live mapped with live predicate;
- /health/ready mapped with ready predicate;
- HealthCheckResponseWriter.WriteAsync.

---

## 5. Database readiness check

DatabaseConnectivityHealthCheck must use CaritasDbContext and Database.CanConnectAsync.

The database readiness check must return:

- healthy when database connectivity succeeds;
- unhealthy when database connectivity fails;
- sanitized description without connection string leakage.

---

## 6. Deployment smoke script

The deployment health smoke script must:

- read CARITAS_DEPLOYMENT_SMOKE_BASE_URL or -BaseUrl;
- skip safely when no base URL is provided unless -Required is set;
- call /health/live;
- call /health/ready;
- call /;
- require HTTP 200;
- require healthy status for live and ready endpoints;
- verify the root endpoint returns service identity;
- reject forbidden tokens in response bodies.

---

## 7. Production go-live impact

Production go-live remains blocked until deployment health smoke is executed against the deployed environment and evidence is attached to the release record.

---

## 8. Non-goals

P3-26E does not implement OpenTelemetry.

P3-26E does not implement cloud alerting.

P3-26E does not add database migrations.

P3-26E does not approve production go-live.

---

## 9. Acceptance criteria

P3-26E is complete when:

- health response writer exists;
- database connectivity health check exists;
- /health/live returns JSON;
- /health/ready returns JSON;
- ready endpoint validates database connectivity;
- deployment health smoke script exists;
- integration tests validate live and ready endpoints;
- repository governance validation includes the health smoke verifier;
- dotnet build and dotnet test pass.