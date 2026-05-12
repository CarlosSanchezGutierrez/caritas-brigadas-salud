using System.Text.RegularExpressions;
using Caritas.Brigadas.Application.Security;
using Xunit;

namespace Caritas.Brigadas.Api.Tests.Security;

public sealed class SeedEndpointAuthorizationContractTests
{
    [Fact]
    public void SecuritySeedEndpoint_RequiresRolesAssignPermission()
    {
        var source = ReadControllerSource("SecuritySeedController.cs");

        AssertSeedEndpointPolicy(
            source,
            "seed-defaults",
            "PermissionCodes.RolesAssign");
    }

    [Fact]
    public void ServicesSeedEndpoint_RequiresServicesSeedPermission()
    {
        var source = ReadControllerSource("ServicesController.cs");

        AssertSeedEndpointPolicy(
            source,
            "seed-defaults",
            "PermissionCodes.ServicesSeed");
    }

    [Fact]
    public void FormTemplatesSeedEndpoint_RequiresFormTemplatesSeedPermission()
    {
        var source = ReadControllerSource("FormTemplatesController.cs");

        AssertSeedEndpointPolicy(
            source,
            "seed-defaults",
            "PermissionCodes.FormTemplatesSeed");
    }

    [Fact]
    public void SeedEndpointPermissions_AreTenantScoped()
    {
        Assert.Contains(PermissionCodes.RolesAssign, PermissionCodes.TenantScoped);
        Assert.Contains(PermissionCodes.ServicesSeed, PermissionCodes.TenantScoped);
        Assert.Contains(PermissionCodes.FormTemplatesSeed, PermissionCodes.TenantScoped);
    }

    private static void AssertSeedEndpointPolicy(
        string source,
        string routeFragment,
        string expectedPolicy)
    {
        var seedEndpoint = Regex.Match(
            source,
            $@"^\s*\[HttpPost\(""[^""]*{Regex.Escape(routeFragment)}[^""]*""\)\](?<attributes>.*?)(public\s+async\s+Task<IActionResult>)",
            RegexOptions.Singleline | RegexOptions.Multiline);

        Assert.True(seedEndpoint.Success, $"Seed endpoint containing route fragment '{routeFragment}' was not found.");

        var attributes = seedEndpoint.Groups["attributes"].Value;

        Assert.Contains(
            $"[Authorize(Policy = {expectedPolicy})]",
            attributes,
            StringComparison.Ordinal);
    }

    private static string ReadControllerSource(string fileName)
    {
        var repositoryRoot = FindRepositoryRoot();

        var sourcePath = Path.Combine(
            repositoryRoot,
            "services",
            "api-dotnet",
            "src",
            "Caritas.Brigadas.Api",
            "Controllers",
            fileName);

        return File.ReadAllText(sourcePath);
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