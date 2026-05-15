using System.Text.RegularExpressions;
using Xunit;

namespace Caritas.Brigadas.Api.Tests.Security;

public sealed class P3PreMainSecurityReviewFindingsTests
{
    [Fact]
    public void RequestTelemetryMiddleware_NormalizesRequestMethodBeforeLogging()
    {
        var source = File.ReadAllText(GetApiSourcePath("Middleware", "RequestTelemetryMiddleware.cs"));

        Assert.Contains("SanitizeForLog(NormalizeHttpMethodForLog(context.Request.Method))", source, StringComparison.Ordinal);
        Assert.Contains("private static string NormalizeHttpMethodForLog(string? method)", source, StringComparison.Ordinal);
        Assert.Contains("normalizedMethod is \"GET\"", source, StringComparison.Ordinal);
        Assert.Contains("private static string SanitizeForLog(string? value)", source, StringComparison.Ordinal);
        Assert.Contains("char.IsControl", source, StringComparison.Ordinal);
        Assert.Contains("return SanitizeForLog(rawPath);", source, StringComparison.Ordinal);
        Assert.DoesNotContain("var httpMethod = context.Request.Method;", source, StringComparison.Ordinal);
        Assert.DoesNotContain("var httpMethod = SanitizeForLog(context.Request.Method);", source, StringComparison.Ordinal);
    }

    [Fact]
    public void HttpAuditLogger_UsesSharedCorrelationId()
    {
        var source = File.ReadAllText(GetApiSourcePath("Audit", "HttpAuditLogger.cs"));

        Assert.Contains("using Caritas.Brigadas.Api.Extensions;", source, StringComparison.Ordinal);
        Assert.Contains("CorrelationId = httpContext?.GetCorrelationId(),", source, StringComparison.Ordinal);
        Assert.DoesNotContain("CorrelationId = httpContext?.TraceIdentifier,", source, StringComparison.Ordinal);
    }

    [Fact]
    public void CaritasDbContext_AppliesAuditLogConfiguration()
    {
        var source = File.ReadAllText(GetInfrastructureSourcePath("Persistence", "CaritasDbContext.cs"));

        Assert.Contains(
            "using Caritas.Brigadas.Infrastructure.Persistence.Configurations;",
            source,
            StringComparison.Ordinal);

        Assert.Contains(
            "modelBuilder.ApplyConfiguration(new AuditLogConfiguration());",
            source,
            StringComparison.Ordinal);
    }

    [Fact]
    public void AuditLogConfiguration_StoresFullAcceptedCorrelationIds()
    {
        var source = File.ReadAllText(GetInfrastructureSourcePath(
            "Persistence",
            "Configurations",
            "AuditLogConfiguration.cs"));

        Assert.Matches(
            @"builder\.Property\(auditLog => auditLog\.CorrelationId\)\s*\r?\n\s*\.HasMaxLength\(128\);",
            source);

        Assert.DoesNotMatch(
            @"builder\.Property\(auditLog => auditLog\.CorrelationId\)\s*\r?\n\s*\.HasMaxLength\(100\);",
            source);
    }

    [Fact]
    public void AuditLogMigrationHistory_PreservesOriginalAndAddsFollowUpWideningMigration()
    {
        var migrationRoot = Path.Combine(
            FindRepositoryRoot(),
            "services",
            "api-dotnet",
            "src",
            "Caritas.Brigadas.Infrastructure",
            "Persistence",
            "Migrations");

        Assert.True(
            File.Exists(Path.Combine(migrationRoot, "20260515055019_ApplyAuditLogConfiguration.cs")),
            "The original ApplyAuditLogConfiguration migration must be preserved.");

        Assert.True(
            File.Exists(Path.Combine(migrationRoot, "20260515055019_ApplyAuditLogConfiguration.Designer.cs")),
            "The original ApplyAuditLogConfiguration designer must be preserved.");

        var widenMigration = Directory.GetFiles(
                migrationRoot,
                "*WidenAuditLogCorrelationIdTo128.cs",
                SearchOption.TopDirectoryOnly)
            .SingleOrDefault(file => !file.EndsWith(".Designer.cs", StringComparison.Ordinal));

        Assert.False(string.IsNullOrWhiteSpace(widenMigration));

        var widenMigrationSource = File.ReadAllText(widenMigration!);

        Assert.Contains("CorrelationId", widenMigrationSource, StringComparison.Ordinal);
        Assert.Contains("maxLength: 128", widenMigrationSource, StringComparison.Ordinal);
    }

    [Fact]
    public void SecurityReviewVerifier_IsWiredIntoGovernance()
    {
        var verifier = File.ReadAllText(GetScriptPath("verify-p3-pre-main-security-review-findings.ps1"));
        var governance = File.ReadAllText(GetScriptPath("validate-repo-governance-baseline.ps1"));

        Assert.Contains("P3 pre-main security review findings verification passed.", verifier, StringComparison.Ordinal);
        Assert.Contains("verify-p3-pre-main-security-review-findings.ps1", governance, StringComparison.Ordinal);
    }

    private static string GetApiSourcePath(params string[] parts)
    {
        return Path.Combine(
            new[]
            {
                FindRepositoryRoot(),
                "services",
                "api-dotnet",
                "src",
                "Caritas.Brigadas.Api"
            }.Concat(parts).ToArray());
    }

    private static string GetInfrastructureSourcePath(params string[] parts)
    {
        return Path.Combine(
            new[]
            {
                FindRepositoryRoot(),
                "services",
                "api-dotnet",
                "src",
                "Caritas.Brigadas.Infrastructure"
            }.Concat(parts).ToArray());
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
