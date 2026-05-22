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
        Assert.Contains("IsValidKnownNetworkPrefixLength(prefix, prefixLength)", program, StringComparison.Ordinal);
        Assert.Contains("AddressFamily.InterNetwork", program, StringComparison.Ordinal);
        Assert.Contains("AddressFamily.InterNetworkV6", program, StringComparison.Ordinal);
        Assert.Contains("prefixLength is >= 0 and <= 32", program, StringComparison.Ordinal);
        Assert.Contains("prefixLength is >= 0 and <= 128", program, StringComparison.Ordinal);
        Assert.Contains("new System.Net.IPNetwork(prefix, prefixLength)", program, StringComparison.Ordinal);
    }

    [Fact]
    public void WebAuthHeaders_SendBearerTokenWhenTokenExistsAndAllowAnonymousWhenMissing()
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
        Assert.Contains("return {} satisfies Record<string, string>;", authHeaders, StringComparison.Ordinal);
        Assert.DoesNotContain("OIDC access token is required", authHeaders, StringComparison.Ordinal);
        Assert.DoesNotContain("throw new Error(", authHeaders, StringComparison.Ordinal);
        Assert.DoesNotContain("const storageCandidates = [window.sessionStorage, window.localStorage];", authHeaders, StringComparison.Ordinal);
    }

    [Fact]
    public void RequestTelemetryMiddleware_UsesCodeQlFriendlyConstantLogValues()
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
        Assert.Contains("SanitizeForLog(context.GetCorrelationId())", telemetry, StringComparison.Ordinal);

        Assert.DoesNotContain("SanitizePath(context.Request.Path)", telemetry, StringComparison.Ordinal);
        Assert.DoesNotContain("context.TraceIdentifier);", telemetry, StringComparison.Ordinal);
        Assert.DoesNotContain("NormalizeHttpMethodForLog", telemetry, StringComparison.Ordinal);
        Assert.DoesNotContain("rawPath.Split", telemetry, StringComparison.Ordinal);
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
