using Xunit;

namespace Caritas.Brigadas.Api.Tests.Security;

public sealed class P35TelemetryLogSanitizationTests
{
    [Fact]
    public void RequestTelemetryMiddleware_UsesConstantHttpMethodsAndEndpointTemplatesForLogs()
    {
        var telemetry = File.ReadAllText(GetRepoPath(
            "services",
            "api-dotnet",
            "src",
            "Caritas.Brigadas.Api",
            "Middleware",
            "RequestTelemetryMiddleware.cs"));

        Assert.Contains("using Caritas.Brigadas.Api.Extensions;", telemetry, StringComparison.Ordinal);
        Assert.Contains("using Microsoft.AspNetCore.Mvc.Controllers;", telemetry, StringComparison.Ordinal);
        Assert.Contains("context.GetCorrelationId()", telemetry, StringComparison.Ordinal);
        Assert.Contains("GetSafeEndpointRouteForLog(context.GetEndpoint())", telemetry, StringComparison.Ordinal);
        Assert.Contains("ControllerActionDescriptor", telemetry, StringComparison.Ordinal);
        Assert.Contains("ClassifyEndpointTemplateForLog", telemetry, StringComparison.Ordinal);
        Assert.Contains("HttpMethods.IsGet(method)", telemetry, StringComparison.Ordinal);
        Assert.Contains("/api/v1/[sensitive-resource]", telemetry, StringComparison.Ordinal);
        Assert.Contains("/api/v1/organizations/[id]", telemetry, StringComparison.Ordinal);
        Assert.Contains("/api/v1/organizations/[id]/reports/[segment]", telemetry, StringComparison.Ordinal);
        Assert.Contains("SanitizeForLog(context.GetCorrelationId())", telemetry, StringComparison.Ordinal);

        Assert.DoesNotContain("SanitizePath(context.Request.Path)", telemetry, StringComparison.Ordinal);
        Assert.DoesNotContain("context.TraceIdentifier);", telemetry, StringComparison.Ordinal);
        Assert.DoesNotContain("NormalizeHttpMethodForLog", telemetry, StringComparison.Ordinal);
        Assert.DoesNotContain("rawPath.Split", telemetry, StringComparison.Ordinal);
        Assert.DoesNotContain("return SanitizeForLog(rawPath);", telemetry, StringComparison.Ordinal);
        Assert.DoesNotContain("if (MaxEndpointSegments <= 0)", telemetry, StringComparison.Ordinal);
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
