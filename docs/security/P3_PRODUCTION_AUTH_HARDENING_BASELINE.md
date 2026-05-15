# P3 Production Authentication Hardening Baseline

Status: active
Scope: production authentication and development header hardening
Target phase: P3-26B
Depends on: P3-26A production deployment readiness baseline

---

## 1. Purpose

P3-26B formalizes authentication hardening before production deployment.

The backend may use development headers only in the Development environment for integration testing and local developer workflows.

Development headers must never be accepted as a production authentication mechanism.

---

## 2. Production authentication rule

Production authentication must use JWT Bearer authentication.

Production must not use:

- Authentication:Mode = Development;
- Authentication:Mode = Disabled;
- X-Dev-User-Id;
- X-Dev-Organization-Id;
- X-Dev-Roles;
- X-Dev-Permissions;
- X-Dev-Name;
- X-Dev-Email.

---

## 3. Current implementation evidence

The API startup pipeline must include:

- ValidateProductionConfiguration before service registration;
- AddCaritasAuthenticationOptions;
- AddConfiguredAuthentication;
- AddPermissionAuthorization;
- AddOrganizationAccessEnforcement;
- UseAuthentication;
- UseAuthorization.

The authentication options must include:

- CaritasAuthenticationOptions.SectionName = Authentication;
- ValidateForEnvironment;
- Development authentication mode is only allowed in Development environment.;
- Disabled authentication mode is not allowed outside Development environment.;
- JWT Bearer authentication requires Authentication:Authority.;
- JWT Bearer authentication requires Authentication:Audience or Authentication:ValidAudiences.

---

## 4. JWT production requirements

Production JWT configuration must provide:

- Authentication:Mode = JwtBearer;
- Authentication:Authority;
- Authentication:Audience or Authentication:ValidAudiences;
- issuer validation;
- audience validation;
- lifetime validation;
- signing key validation;
- RoleClaimType = CurrentUserClaimTypes.RoleCode.

---

## 5. Development header boundary

Development headers are allowed only for:

- local development;
- automated integration tests;
- Development environment only.

Development headers must be treated as test scaffolding.

They are not a production auth protocol.

---

## 6. Required tests

The test suite must validate:

- Development authentication mode outside Development returns validation errors;
- Disabled authentication mode outside Development returns validation errors;
- JWT Bearer mode without Authority returns validation errors;
- JWT Bearer mode without Audience or ValidAudiences returns validation errors;
- JWT Bearer mode with Authority and Audience returns no validation errors;
- Program.cs includes production configuration validation before configured authentication.

---

## 7. Production go-live impact

Production go-live remains blocked unless:

- P3-26B is complete;
- Authentication:Mode is JwtBearer in production;
- no production deployment uses X-Dev-* headers;
- production JWT authority and audience values are configured through environment or secret manager;
- endpoint permission tests remain passing.

---

## 8. Non-goals

P3-26B does not integrate with a real identity provider.

P3-26B does not issue tokens.

P3-26B does not validate Entra ID/Auth0 tenant-specific settings.

P3-26B does not remove Development authentication from tests.

P3-26B does not approve production go-live.

---

## 9. Acceptance criteria

P3-26B is complete when:

- this auth hardening baseline exists;
- the auth hardening verifier exists;
- the auth hardening contract tests exist;
- production readiness baseline references P3-26B;
- repository governance validation includes the auth hardening verifier;
- dotnet build and dotnet test pass.