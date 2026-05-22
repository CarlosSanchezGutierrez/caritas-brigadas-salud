using Xunit;

namespace Caritas.Brigadas.Api.Tests.Security;

public sealed class P35MainPromotionSecurityBlockerTests
{
    [Fact]
    public void Program_DoesNotTrustAllForwardedHeaders()
    {
        var program = File.ReadAllText(GetRepoPath(
            "services",
            "api-dotnet",
            "src",
            "Caritas.Brigadas.Api",
            "Program.cs"));

        Assert.DoesNotContain("options.KnownIPNetworks.Clear();", program, StringComparison.Ordinal);
        Assert.DoesNotContain("options.KnownProxies.Clear();", program, StringComparison.Ordinal);
        Assert.Contains("ReverseProxy:ForwardedHeaders:KnownProxies", program, StringComparison.Ordinal);
        Assert.Contains("ReverseProxy:ForwardedHeaders:KnownIPNetworks", program, StringComparison.Ordinal);
        Assert.Contains("IPAddress.TryParse", program, StringComparison.Ordinal);
        Assert.Contains("options.KnownIPNetworks.Add", program, StringComparison.Ordinal);
        Assert.Contains("new System.Net.IPNetwork", program, StringComparison.Ordinal);
        Assert.Contains("IsValidKnownNetworkPrefixLength(prefix, prefixLength)", program, StringComparison.Ordinal);
        Assert.Contains("AddressFamily.InterNetwork", program, StringComparison.Ordinal);
        Assert.Contains("AddressFamily.InterNetworkV6", program, StringComparison.Ordinal);
        Assert.Contains("prefixLength is >= 0 and <= 32", program, StringComparison.Ordinal);
        Assert.Contains("prefixLength is >= 0 and <= 128", program, StringComparison.Ordinal);
    }

    [Fact]
    public void WebAuthHeaders_SendBearerWhenTokenExistsAndAllowAnonymousWhenMissing()
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
        Assert.Contains("return {} satisfies Record<string, string>;", authHeaders, StringComparison.Ordinal);
        Assert.Contains("readOidcAccessTokenFromBrowserStorage", authHeaders, StringComparison.Ordinal);
        Assert.Contains("normalizeBearerToken", authHeaders, StringComparison.Ordinal);
        Assert.Contains("readBrowserStorageItem", authHeaders, StringComparison.Ordinal);
        Assert.Contains("window[storageName]", authHeaders, StringComparison.Ordinal);
        Assert.DoesNotContain("OIDC access token is required", authHeaders, StringComparison.Ordinal);
        Assert.DoesNotContain("throw new Error(", authHeaders, StringComparison.Ordinal);
        Assert.DoesNotContain("const storageCandidates = [window.sessionStorage, window.localStorage];", authHeaders, StringComparison.Ordinal);
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
