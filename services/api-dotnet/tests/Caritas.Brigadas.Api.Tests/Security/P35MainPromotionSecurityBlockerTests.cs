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
        Assert.Contains("ReverseProxy:ForwardedHeaders:KnownNetworks", program, StringComparison.Ordinal);
        Assert.Contains("IPAddress.TryParse", program, StringComparison.Ordinal);
        Assert.Contains("new IPNetwork", program, StringComparison.Ordinal);
    }

    [Fact]
    public void WebAuthHeaders_DoNotReturnEmptyHeadersInOidcMode()
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
        Assert.Contains("OIDC access token is required", authHeaders, StringComparison.Ordinal);
        Assert.Contains("readOidcAccessTokenFromBrowserStorage", authHeaders, StringComparison.Ordinal);
        Assert.Contains("normalizeBearerToken", authHeaders, StringComparison.Ordinal);
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
