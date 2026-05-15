using Xunit;

namespace Caritas.Brigadas.Api.Tests.Security;

public sealed class P3ProductionCorsRateLimitingContractTests
{
    [Fact]
    public void Program_ConfiguresCorsAndRateLimitingRuntime()
    {
        var source = File.ReadAllText(GetApiSourcePath("Program.cs"));

        var requiredTokens = new[]
        {
            "const string CorsPolicyName = \"ConfiguredOrigins\";",
            "builder.Services.AddCors",
            "Cors:AllowedOrigins",
            "WithOrigins(allowedOrigins)",
            "app.UseCors(CorsPolicyName);",
            "Security:RateLimiting:Enabled",
            "Security:RateLimiting:PermitLimit",
            "Security:RateLimiting:WindowMinutes",
            "Security:RateLimiting:QueueLimit",
            "builder.Services.AddRateLimiter",
            "options.RejectionStatusCode = StatusCodes.Status429TooManyRequests;",
            "PartitionedRateLimiter.Create<HttpContext, string>",
            "RateLimitPartition.GetFixedWindowLimiter",
            "FixedWindowRateLimiterOptions",
            "app.UseRateLimiter();"
        };

        AssertRequiredTokens(source, requiredTokens, "Program.cs CORS/rate limiting runtime");
    }

    [Fact]
    public void ProductionValidation_RejectsUnsafeCorsAndRateLimitingConfiguration()
    {
        var source = File.ReadAllText(GetApiExtensionPath("ProductionConfigurationValidationExtensions.cs"));

        var requiredTokens = new[]
        {
            "ValidateProductionCors(configuration);",
            "ValidateProductionRateLimiting(configuration);",
            "private static void ValidateProductionCors",
            "private static void ValidateProductionRateLimiting",
            "private static bool IsUnsafeCorsOrigin",
            "Security:RateLimiting:Enabled",
            "Security:RateLimiting:PermitLimit",
            "Security:RateLimiting:WindowMinutes",
            "Security:RateLimiting:QueueLimit",
            "Production requires at least one explicit Cors:AllowedOrigins entry.",
            "Production CORS origins must be explicit HTTPS origins and cannot use localhost, loopback addresses, or wildcards.",
            "Production requires Security:RateLimiting:Enabled to be true.",
            "Production requires Security:RateLimiting:PermitLimit to be greater than zero.",
            "Production requires Security:RateLimiting:WindowMinutes to be greater than zero.",
            "Production requires Security:RateLimiting:QueueLimit to be zero or greater."
        };

        AssertRequiredTokens(source, requiredTokens, "ProductionConfigurationValidationExtensions CORS/rate limiting");
    }

    [Fact]
    public void ProductionValidationTests_CoverCorsAndRateLimitingFailures()
    {
        var source = File.ReadAllText(GetSecurityTestPath("ProductionConfigurationValidationTests.cs"));

        var requiredTokens = new[]
        {
            "ValidateProductionConfiguration_Throws_WhenProductionHasNoCorsOrigins",
            "ValidateProductionConfiguration_Throws_WhenProductionCorsOriginIsUnsafe",
            "ValidateProductionConfiguration_Throws_WhenProductionDisablesRateLimiting",
            "ValidateProductionConfiguration_Throws_WhenProductionRateLimitingValuesAreInvalid",
            "Security:RateLimiting:Enabled",
            "Security:RateLimiting:PermitLimit",
            "Security:RateLimiting:WindowMinutes",
            "Security:RateLimiting:QueueLimit",
            "https://localhost:3000",
            "http://brigadas.caritas.example.org",
            "*",
            "not-a-uri"
        };

        AssertRequiredTokens(source, requiredTokens, "ProductionConfigurationValidationTests CORS/rate limiting");
    }

    [Fact]
    public void ProductionCorsRateLimitingBaseline_DefinesPublicExposureHardeningScope()
    {
        var source = File.ReadAllText(GetOperationsDocPath("P3_PRODUCTION_CORS_RATE_LIMITING_BASELINE.md"));

        var requiredTokens = new[]
        {
            "P3 Production CORS and Rate Limiting Baseline",
            "Production requires explicit Cors:AllowedOrigins.",
            "Production requires Security:RateLimiting:Enabled to be true.",
            "Security:RateLimiting:PermitLimit greater than zero",
            "Security:RateLimiting:WindowMinutes greater than zero",
            "Security:RateLimiting:QueueLimit zero or greater",
            "Runtime evidence",
            "Production validation evidence",
            "Production go-live remains blocked",
            "Acceptance criteria"
        };

        AssertRequiredTokens(source, requiredTokens, "P3 production CORS/rate limiting baseline");
    }

    [Fact]
    public void RepositoryGovernanceBaseline_RunsProductionCorsRateLimitingVerifier()
    {
        var source = File.ReadAllText(GetScriptPath("validate-repo-governance-baseline.ps1"));

        Assert.Contains(
            "verify-p3-production-cors-rate-limiting.ps1",
            source,
            StringComparison.Ordinal);
    }

    private static void AssertRequiredTokens(
        string source,
        IReadOnlyCollection<string> requiredTokens,
        string label)
    {
        var failures = requiredTokens
            .Where(token => !source.Contains(token, StringComparison.Ordinal))
            .Select(token => $"{label} is missing required token: {token}")
            .ToArray();

        Assert.True(
            failures.Length == 0,
            $"{label} is incomplete." +
            Environment.NewLine +
            string.Join(Environment.NewLine, failures));
    }

    private static string GetApiSourcePath(string fileName)
    {
        return Path.Combine(
            FindRepositoryRoot(),
            "services",
            "api-dotnet",
            "src",
            "Caritas.Brigadas.Api",
            fileName);
    }

    private static string GetApiExtensionPath(string fileName)
    {
        return Path.Combine(
            FindRepositoryRoot(),
            "services",
            "api-dotnet",
            "src",
            "Caritas.Brigadas.Api",
            "Extensions",
            fileName);
    }

    private static string GetSecurityTestPath(string fileName)
    {
        return Path.Combine(
            FindRepositoryRoot(),
            "services",
            "api-dotnet",
            "tests",
            "Caritas.Brigadas.Api.Tests",
            "Security",
            fileName);
    }

    private static string GetOperationsDocPath(string fileName)
    {
        return Path.Combine(
            FindRepositoryRoot(),
            "docs",
            "operations",
            fileName);
    }

    private static string GetScriptPath(string fileName)
    {
        return Path.Combine(
            FindRepositoryRoot(),
            "scripts",
            fileName);
    }

    private static string FindRepositoryRoot()
    {
        var directory = new DirectoryInfo(AppContext.BaseDirectory);

        while (directory is not null)
        {
            if (Directory.Exists(Path.Combine(directory.FullName, ".git")))
            {
                return directory.FullName;
            }

            directory = directory.Parent;
        }

        throw new DirectoryNotFoundException("Repository root with .git directory was not found.");
    }
}
