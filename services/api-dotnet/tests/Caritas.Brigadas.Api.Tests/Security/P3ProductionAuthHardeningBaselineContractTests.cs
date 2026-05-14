using Caritas.Brigadas.Api.Options;
using Xunit;

namespace Caritas.Brigadas.Api.Tests.Security;

public sealed class P3ProductionAuthHardeningBaselineContractTests
{
    [Fact]
    public void AuthenticationOptions_RejectDevelopmentModeOutsideDevelopment()
    {
        var options = new CaritasAuthenticationOptions
        {
            Mode = CaritasAuthenticationModes.Development
        };

        var errors = options.ValidateForEnvironment("Production");

        Assert.Contains(
            "Development authentication mode is only allowed in Development environment.",
            errors);
    }

    [Fact]
    public void AuthenticationOptions_RejectDisabledModeOutsideDevelopment()
    {
        var options = new CaritasAuthenticationOptions
        {
            Mode = CaritasAuthenticationModes.Disabled
        };

        var errors = options.ValidateForEnvironment("Production");

        Assert.Contains(
            "Disabled authentication mode is not allowed outside Development environment.",
            errors);
    }

    [Fact]
    public void AuthenticationOptions_RejectJwtBearerWithoutAuthority()
    {
        var options = new CaritasAuthenticationOptions
        {
            Mode = CaritasAuthenticationModes.JwtBearer,
            Audience = "api://caritas-brigadas"
        };

        var errors = options.ValidateForEnvironment("Production");

        Assert.Contains(
            "JWT Bearer authentication requires Authentication:Authority.",
            errors);
    }

    [Fact]
    public void AuthenticationOptions_RejectJwtBearerWithoutAudienceOrValidAudiences()
    {
        var options = new CaritasAuthenticationOptions
        {
            Mode = CaritasAuthenticationModes.JwtBearer,
            Authority = "https://login.example.com/tenant"
        };

        var errors = options.ValidateForEnvironment("Production");

        Assert.Contains(
            "JWT Bearer authentication requires Authentication:Audience or Authentication:ValidAudiences.",
            errors);
    }

    [Fact]
    public void AuthenticationOptions_AcceptJwtBearerWithAuthorityAndAudience()
    {
        var options = new CaritasAuthenticationOptions
        {
            Mode = CaritasAuthenticationModes.JwtBearer,
            Authority = "https://login.example.com/tenant",
            Audience = "api://caritas-brigadas"
        };

        var errors = options.ValidateForEnvironment("Production");

        Assert.Empty(errors);
    }

    [Fact]
    public void Program_ValidatesProductionConfigurationBeforeConfiguredAuthentication()
    {
        var source = File.ReadAllText(GetApiSourcePath("Program.cs"));

        var validateIndex = source.IndexOf(
            "builder.ValidateProductionConfiguration();",
            StringComparison.Ordinal);

        var configuredAuthIndex = source.IndexOf(
            "builder.Services.AddConfiguredAuthentication(builder.Configuration, builder.Environment);",
            StringComparison.Ordinal);

        Assert.True(validateIndex >= 0, "Program.cs must call ValidateProductionConfiguration.");
        Assert.True(configuredAuthIndex >= 0, "Program.cs must call AddConfiguredAuthentication.");
        Assert.True(
            validateIndex < configuredAuthIndex,
            "Program.cs must validate production configuration before configured authentication registration.");
    }

    [Fact]
    public void ProductionAuthHardeningBaseline_DefinesDevelopmentHeaderBoundary()
    {
        var source = File.ReadAllText(GetSecurityDocPath("P3_PRODUCTION_AUTH_HARDENING_BASELINE.md"));

        var requiredTokens = new[]
        {
            "P3 Production Authentication Hardening Baseline",
            "Production authentication must use JWT Bearer authentication.",
            "Authentication:Mode = Development",
            "Authentication:Mode = Disabled",
            "X-Dev-User-Id",
            "X-Dev-Organization-Id",
            "X-Dev-Roles",
            "X-Dev-Permissions",
            "Authentication:Mode = JwtBearer",
            "Development headers must be treated as test scaffolding.",
            "P3-26B is complete",
            "Acceptance criteria"
        };

        AssertRequiredTokens(source, requiredTokens, "P3 production auth hardening baseline");
    }

    [Fact]
    public void ProductionAuthHardeningVerifier_RequiresRuntimeEvidence()
    {
        var source = File.ReadAllText(GetScriptPath("verify-p3-production-auth-hardening-baseline.ps1"));

        var requiredTokens = new[]
        {
            "P3 production auth hardening baseline verification passed.",
            "P3_PRODUCTION_AUTH_HARDENING_BASELINE.md",
            "P3_PRODUCTION_DEPLOYMENT_READINESS_BASELINE.md",
            "Program.cs",
            "CaritasAuthenticationOptions.cs",
            "ConfiguredAuthenticationServiceExtensions.cs",
            "builder.ValidateProductionConfiguration();",
            "builder.Services.AddConfiguredAuthentication(builder.Configuration, builder.Environment);",
            "ValidateIssuer = true",
            "ValidateAudience = true",
            "ValidateLifetime = true",
            "ValidateIssuerSigningKey = true"
        };

        AssertRequiredTokens(source, requiredTokens, "P3 production auth hardening verifier");
    }

    [Fact]
    public void RepositoryGovernanceBaseline_RunsProductionAuthHardeningVerifier()
    {
        var source = File.ReadAllText(GetScriptPath("validate-repo-governance-baseline.ps1"));

        Assert.Contains(
            "verify-p3-production-auth-hardening-baseline.ps1",
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

    private static string GetSecurityDocPath(string fileName)
    {
        return Path.Combine(
            FindRepositoryRoot(),
            "docs",
            "security",
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
