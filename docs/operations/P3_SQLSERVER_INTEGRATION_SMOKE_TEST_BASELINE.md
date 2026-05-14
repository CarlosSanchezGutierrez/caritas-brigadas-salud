# P3 SQL Server Integration Smoke Test Baseline

Status: active
Scope: SQL Server integration smoke testing
Target phase: P3-26C
Depends on: P3-26B production authentication hardening baseline

---

## 1. Purpose

P3-26C adds a real SQL Server smoke test entry point.

The goal is to validate that the backend can build, resolve the EF Core design-time context, list migrations, and optionally apply migrations against a configured SQL Server smoke database.

---

## 2. Execution model

The SQL Server smoke test is intentionally opt-in.

The script must not require SQL Server during normal CI unless an explicit connection string is provided.

The smoke script reads the connection string from:

- CARITAS_SQLSERVER_SMOKE_CONNECTION;
- or an explicit -ConnectionString parameter.

The script passes the selected connection string to EF Core through:

- CARITAS_SQLSERVER_CONNECTION.

This matches the design-time DbContext factory contract.

---

## 3. Safety rule

The SQL Server smoke test must not run accidentally against production.

The smoke script must require the target connection string or database name to contain one of these safety markers:

- Smoke;
- Test;
- Local;
- Dev.

If a non-smoke database must be used, the operator must pass -AllowNonSmokeDatabase explicitly.

---

## 4. Smoke test steps

The SQL Server smoke script must perform:

1. validate repository paths;
2. validate connection string presence when -Required is used;
3. reject unsafe database names unless explicitly allowed;
4. set CARITAS_SQLSERVER_CONNECTION;
5. dotnet tool restore for the local tool manifest;
6. dotnet build with warnaserror;
7. dotnet ef migrations list for CaritasDbContext;
8. dotnet ef database update for CaritasDbContext unless -SkipDatabaseUpdate is used.

---

## 5. Required commands

The smoke script must use:

- dotnet tool restore;
- dotnet build;
- dotnet ef migrations list;
- dotnet ef database update;
- --project src/Caritas.Brigadas.Infrastructure;
- --startup-project src/Caritas.Brigadas.Api;
- --context CaritasDbContext.

---

## 6. CI behavior

Normal repository CI may verify the smoke script and baseline metadata without running a live SQL Server database.

A live SQL Server smoke test should run in a dedicated environment with CARITAS_SQLSERVER_SMOKE_CONNECTION configured.

---

## 7. Production go-live impact

Production go-live remains blocked until a SQL Server smoke execution has been performed against a controlled staging or smoke database and the evidence is attached to the deployment record.

---

## 8. Required evidence

A successful SQL Server smoke execution must record:

- git commit SHA;
- connection target classification;
- environment name;
- database name;
- migration list result;
- database update result;
- operator;
- timestamp UTC;
- result status.

---

## 9. Non-goals

P3-26C does not run against production.

P3-26C does not approve go-live.

P3-26C does not replace full staging validation.

P3-26C does not replace backup and restore testing.

P3-26C does not execute the smoke test automatically in every CI run.

---

## 10. Acceptance criteria

P3-26C is complete when:

- this SQL Server smoke test baseline exists;
- the SQL Server smoke script exists;
- the SQL Server smoke verifier exists;
- the SQL Server smoke contract test exists;
- production deployment readiness references P3-26C;
- repository governance validation includes the SQL Server smoke verifier;
- dotnet build and dotnet test pass.