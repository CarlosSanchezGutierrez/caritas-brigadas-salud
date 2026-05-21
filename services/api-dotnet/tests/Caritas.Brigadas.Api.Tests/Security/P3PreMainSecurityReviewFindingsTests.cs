using Xunit;

namespace Caritas.Brigadas.Api.Tests.Security;

public sealed class P3PreMainSecurityReviewFindingsTests
{
    [Fact]
    public void Program_RestrictsForwardedHeadersToConfiguredTrustedProxies()
    {
        var program = File.ReadAllText(GetRepoPath(
            "services",
            "api-dotnet",
            "src",
            "Caritas.Brigadas.Api",
            "Program.cs"));

        Assert.DoesNotContain("options.KnownIPNetworks.Clear();", program, StringComparison.Ordinal);
        Assert.DoesNotContain("options.KnownProxies.Clear();", program, StringComparison.Ordinal);
        Assert.DoesNotContain("Microsoft.AspNetCore.HttpOverrides.IPNetwork", program, StringComparison.Ordinal);
        Assert.DoesNotContain("options.KnownNetworks", program, StringComparison.Ordinal);
        Assert.Contains("ReverseProxy:ForwardedHeaders:KnownProxies", program, StringComparison.Ordinal);
        Assert.Contains("ReverseProxy:ForwardedHeaders:KnownIPNetworks", program, StringComparison.Ordinal);
        Assert.Contains("new System.Net.IPNetwork(prefix, prefixLength)", program, StringComparison.Ordinal);
    }

    [Fact]
    public void WebAuthHeaders_SendBearerTokenInOidcMode()
    {
        var authHeaders = File.ReadAllText(GetRepoPath(
            "apps",
            "web-next",
            "src",
            "lib",
            "auth-headers.ts"));

        Assert.DoesNotContain("if (AUTH_MODE === \"oidc\") {\n    return {};", authHeaders, StringComparison.Ordinal);
        Assert.Contains("Authorization", authHeaders, StringComparison.Ordinal);
        Assert.Contains("Bearer", authHeaders, StringComparison.Ordinal);
        Assert.Contains("readBrowserStorageItem", authHeaders, StringComparison.Ordinal);
        Assert.DoesNotContain("const storageCandidates = [window.sessionStorage, window.localStorage];", authHeaders, StringComparison.Ordinal);
    }

    [Fact]
    public void RequestTelemetryMiddleware_UsesSafeAllowlistedLogValues()
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
        Assert.Contains("SanitizePath(context.Request.Path)", telemetry, StringComparison.Ordinal);
        Assert.Contains("GetSafeHttpMethodForLog(context.Request.Method)", telemetry, StringComparison.Ordinal);
        Assert.Contains("AllowedHttpMethodsForLog", telemetry, StringComparison.Ordinal);
        Assert.Contains("AllowedPathSegmentsForLog", telemetry, StringComparison.Ordinal);
        Assert.Contains("/api/v1/[sensitive-resource]", telemetry, StringComparison.Ordinal);
        Assert.Contains("[segment]", telemetry, StringComparison.Ordinal);
        Assert.Contains("[id]", telemetry, StringComparison.Ordinal);
        Assert.Contains("builder.Append('_')", telemetry, StringComparison.Ordinal);
        Assert.Contains("char.IsLetterOrDigit", telemetry, StringComparison.Ordinal);

        Assert.DoesNotContain("GetSafeHttpMethodForLog(context.Request.Method)", telemetry, StringComparison.Ordinal);
        Assert.DoesNotContain("GetSafeHttpMethodForLog", telemetry, StringComparison.Ordinal);
        Assert.DoesNotContain("normalizedMethod is \"GET\"", telemetry, StringComparison.Ordinal);
        Assert.DoesNotContain("return SanitizeForLog(rawPath);", telemetry, StringComparison.Ordinal);
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
