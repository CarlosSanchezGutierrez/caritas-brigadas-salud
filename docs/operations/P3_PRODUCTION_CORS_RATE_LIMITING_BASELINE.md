# P3 Production CORS and Rate Limiting Baseline

Status: active
Scope: production CORS and rate limiting validation
Target phase: P3-26G
Depends on: P3-26F structured logging and correlation id hardening

---

## 1. Purpose

P3-26G closes the public-exposure hardening baseline for production CORS and global rate limiting.

The backend must reject production startup when CORS or rate limiting configuration is unsafe.

---

## 2. Production CORS requirements

Production requires explicit Cors:AllowedOrigins.

Production CORS origins must:

- be configured explicitly;
- use HTTPS;
- not use localhost;
- not use 127.0.0.1;
- not use ::1;
- not use wildcard origins;
- be absolute URI values.

The development fallback origins are allowed only outside production.

---

## 3. Production rate limiting requirements

Production requires Security:RateLimiting:Enabled to be true.

Production rate limiting must have:

- Security:RateLimiting:PermitLimit greater than zero;
- Security:RateLimiting:WindowMinutes greater than zero;
- Security:RateLimiting:QueueLimit zero or greater;
- StatusCodes.Status429TooManyRequests rejection status;
- PartitionedRateLimiter global limiter;
- FixedWindowRateLimiterOptions.

---

## 4. Runtime evidence

Program.cs must include:

- AddCors;
- Cors:AllowedOrigins;
- WithOrigins(allowedOrigins);
- UseCors(CorsPolicyName);
- AddRateLimiter;
- UseRateLimiter;
- StatusCodes.Status429TooManyRequests;
- PartitionedRateLimiter.Create<HttpContext, string>;
- RateLimitPartition.GetFixedWindowLimiter;
- FixedWindowRateLimiterOptions.

---

## 5. Production validation evidence

ProductionConfigurationValidationExtensions must validate:

- ValidateProductionCors;
- ValidateProductionRateLimiting;
- IsUnsafeCorsOrigin;
- Security:RateLimiting:Enabled;
- Security:RateLimiting:PermitLimit;
- Security:RateLimiting:WindowMinutes;
- Security:RateLimiting:QueueLimit;
- Production requires at least one explicit Cors:AllowedOrigins entry.;
- Production CORS origins must be explicit HTTPS origins and cannot use localhost, loopback addresses, or wildcards.;
- Production requires Security:RateLimiting:Enabled to be true.

---

## 6. Production go-live impact

Production go-live remains blocked unless:

- explicit HTTPS CORS origins are configured;
- localhost and wildcard CORS origins are rejected;
- rate limiting is enabled;
- rate limiting values are valid;
- repository governance passes;
- deployment smoke evidence is attached.

---

## 7. Non-goals

P3-26G does not tune final rate limiting thresholds.

P3-26G does not create WAF rules.

P3-26G does not configure CDN protections.

P3-26G does not approve production go-live.

---

## 8. Acceptance criteria

P3-26G is complete when:

- this production CORS and rate limiting baseline exists;
- the production CORS and rate limiting verifier exists;
- production validation tests cover unsafe CORS origins;
- production validation tests cover disabled rate limiting;
- production validation tests cover invalid rate limiting values;
- repository governance validation includes the CORS/rate limiting verifier;
- dotnet build and dotnet test pass.