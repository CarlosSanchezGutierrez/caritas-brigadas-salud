using Xunit;

namespace Caritas.Brigadas.Api.Tests.Security;

public sealed class P35TelemetryLogSanitizationTests
{
    [Fact]
    public void RequestTelemetryMiddleware_UsesAllowlistedHttpMethodsForLogs()
    {
        var telemetry = File.ReadAllText(GetRepoPath(
            "services",
            "api-dotnet",
            "src",
            "Caritas.Brigadas.Api",
            "Middleware",
            "RequestTelemetryMiddleware.cs"));

        Assert.Contains("AllowedHttpMethodsForLog", telemetry, StringComparison.Ordinal);
        Assert.Contains("GetSafeHttpMethodForLog(context.Request.Method)", telemetry, StringComparison.Ordinal);
        Assert.DoesNotContain("SanitizeForLog(NormalizeHttpMethodForLog(context.Request.Method))", telemetry, StringComparison.Ordinal);
        Assert.DoesNotContain("NormalizeHttpMethodForLog", telemetry, StringComparison.Ordinal);
        Assert.DoesNotContain("var httpMethod = SanitizeForLog", telemetry, StringComparison.Ordinal);
        Assert.DoesNotContain("normalizedMethod is \"GET\"", telemetry, StringComparison.Ordinal);
    }

    [Fact]
    public void RequestTelemetryMiddleware_DoesNotLogRawRequestPath()
    {
        var telemetry = File.ReadAllText(GetRepoPath(
            "services",
            "api-dotnet",
            "src",
            "Caritas.Brigadas.Api",
            "Middleware",
            "RequestTelemetryMiddleware.cs"));

        Assert.Contains("SanitizePath(context.Request.Path)", telemetry, StringComparison.Ordinal);
        Assert.Contains("AllowedPathSegmentsForLog", telemetry, StringComparison.Ordinal);
        Assert.Contains("/api/v1/[sensitive-resource]", telemetry, StringComparison.Ordinal);
        Assert.Contains("[segment]", telemetry, StringComparison.Ordinal);
        Assert.Contains("[id]", telemetry, StringComparison.Ordinal);
        Assert.Contains("MaxEndpointRouteLength", telemetry, StringComparison.Ordinal);
        Assert.Contains("MaxEndpointSegments", telemetry, StringComparison.Ordinal);
        Assert.DoesNotContain("return SanitizeForLog(rawPath);", telemetry, StringComparison.Ordinal);
    }

    [Fact]
    public void RequestTelemetryMiddleware_UsesStrictLogSanitizerAndCorrelationContract()
    {
        var telemetry = File.ReadAllText(GetRepoPath(
            "services",
            "api-dotnet",
            "src",
            "Caritas.Brigadas.Api",
            "Middleware",
            "RequestTelemetryMiddleware.cs"));

        Assert.Contains("using Caritas.Brigadas.Api.Extensions;", telemetry, StringComparison.Ordinal);
        Assert.Contains("context.GetCorrelationId()", telemetry, StringComparison.Ordinal);
        Assert.Contains("char.IsLetterOrDigit", telemetry, StringComparison.Ordinal);
        Assert.Contains("builder.Append('_')", telemetry, StringComparison.Ordinal);
        Assert.DoesNotContain("value.Where(static character => !char.IsControl(character))", telemetry, StringComparison.Ordinal);
    }

    private static string GetRepoPath(params string[] parts)
    {
        return Path.Combine(
            new[]
            {
                FindRepositoryRoot()
            }.Concat(parts).ToArray());
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
