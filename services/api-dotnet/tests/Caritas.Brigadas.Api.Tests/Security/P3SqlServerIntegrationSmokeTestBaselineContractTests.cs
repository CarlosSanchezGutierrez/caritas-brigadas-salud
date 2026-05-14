using Xunit;

namespace Caritas.Brigadas.Api.Tests.Security;

public sealed class P3SqlServerIntegrationSmokeTestBaselineContractTests
{
    [Fact]
    public void SqlServerIntegrationSmokeTestBaseline_DefinesOptInSmokeScope()
    {
        var source = File.ReadAllText(GetOperationsDocPath("P3_SQLSERVER_INTEGRATION_SMOKE_TEST_BASELINE.md"));

        var requiredTokens = new[]
        {
            "P3 SQL Server Integration Smoke Test Baseline",
            "real SQL Server smoke test entry point",
            "CARITAS_SQLSERVER_SMOKE_CONNECTION",
            "CARITAS_SQLSERVER_CONNECTION",
            "dotnet ef migrations list",
            "dotnet ef database update",
            "--project src/Caritas.Brigadas.Infrastructure",
            "--startup-project src/Caritas.Brigadas.Api",
            "--context CaritasDbContext",
            "Production go-live remains blocked",
            "Acceptance criteria"
        };

        AssertRequiredTokens(source, requiredTokens, "P3 SQL Server smoke baseline");
    }

    [Fact]
    public void SqlServerIntegrationSmokeScript_IsOptInAndSafetyGuarded()
    {
        var source = File.ReadAllText(GetScriptPath("run-p3-sqlserver-integration-smoke-test.ps1"));

        var requiredTokens = new[]
        {
            "CARITAS_SQLSERVER_SMOKE_CONNECTION",
            "CARITAS_SQLSERVER_CONNECTION",
            "Required",
            "SkipDatabaseUpdate",
            "AllowNonSmokeDatabase",
            "Refusing to run SQL Server smoke test against a database without a Smoke/Test/Local/Dev marker.",
            "dotnet build",
            "dotnet ef --version",
            "dotnet ef migrations list",
            "dotnet ef database update",
            "--project $InfrastructureProject",
            "--startup-project $StartupProject",
            "--context $Context",
            "P3 SQL Server integration smoke test passed."
        };

        AssertRequiredTokens(source, requiredTokens, "P3 SQL Server smoke script");
    }

    [Fact]
    public void SqlServerIntegrationSmokeVerifier_RequiresDesignTimeFactoryEvidence()
    {
        var source = File.ReadAllText(GetScriptPath("verify-p3-sqlserver-integration-smoke-test-baseline.ps1"));

        var requiredTokens = new[]
        {
            "P3 SQL Server integration smoke test baseline verification passed.",
            "P3_SQLSERVER_INTEGRATION_SMOKE_TEST_BASELINE.md",
            "run-p3-sqlserver-integration-smoke-test.ps1",
            "DesignTimeCaritasDbContextFactory.cs",
            "CARITAS_SQLSERVER_CONNECTION",
            "UseSqlServer",
            "verify-p3-sqlserver-integration-smoke-test-baseline.ps1"
        };

        AssertRequiredTokens(source, requiredTokens, "P3 SQL Server smoke verifier");
    }

    [Fact]
    public void DesignTimeFactory_UsesSqlServerConnectionEnvironmentVariable()
    {
        var source = File.ReadAllText(GetInfrastructurePath("DesignTimeCaritasDbContextFactory.cs"));

        var requiredTokens = new[]
        {
            "CARITAS_SQLSERVER_CONNECTION",
            "UseSqlServer",
            "CaritasDbContext"
        };

        AssertRequiredTokens(source, requiredTokens, "DesignTimeCaritasDbContextFactory");
    }

    [Fact]
    public void RepositoryGovernanceBaseline_RunsSqlServerSmokeVerifier()
    {
        var source = File.ReadAllText(GetScriptPath("validate-repo-governance-baseline.ps1"));

        Assert.Contains(
            "verify-p3-sqlserver-integration-smoke-test-baseline.ps1",
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

    private static string GetInfrastructurePath(string fileName)
    {
        return Path.Combine(
            FindRepositoryRoot(),
            "services",
            "api-dotnet",
            "src",
            "Caritas.Brigadas.Infrastructure",
            "Persistence",
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
