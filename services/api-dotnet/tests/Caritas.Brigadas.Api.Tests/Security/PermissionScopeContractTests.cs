using System.Text.RegularExpressions;
using Caritas.Brigadas.Application.Security;
using Xunit;

namespace Caritas.Brigadas.Api.Tests.Security;

public sealed class PermissionScopeContractTests
{
    [Fact]
    public void PermissionCodes_GlobalOnly_IncludesOrganizationWrite()
    {
        Assert.Contains(PermissionCodes.OrganizationsWrite, PermissionCodes.GlobalOnly);
        Assert.All(PermissionCodes.GlobalOnly, permission =>
            Assert.Contains(permission, PermissionCodes.All));
    }

    [Fact]
    public void PermissionCodes_GlobalOnly_HasNoDuplicates()
    {
        var unique = PermissionCodes.GlobalOnly
            .Distinct(StringComparer.OrdinalIgnoreCase)
            .Count();

        Assert.Equal(PermissionCodes.GlobalOnly.Count, unique);
    }

    [Fact]
    public void SecuritySeedRepository_TenantAdmin_DoesNotReceiveGlobalOrganizationWrite()
    {
        var source = ReadSecuritySeedRepositorySource();

        var adminBlock = Regex.Match(
            source,
            @"\[RoleCodes\.Admin\]\s*=\s*new\[\]\s*\{(?<body>.*?)\}",
            RegexOptions.Singleline);

        Assert.True(adminBlock.Success, "ADMIN permission seed block was not found.");

        Assert.DoesNotContain(
            "\"organizations.write\"",
            adminBlock.Groups["body"].Value,
            StringComparison.OrdinalIgnoreCase);
    }

    [Fact]
    public void SecuritySeedRepository_RemovesGlobalOnlyPermissionsFromNonSuperAdminRoles()
    {
        var source = ReadSecuritySeedRepositorySource();

        Assert.Contains("PermissionCodes.GlobalOnly", source, StringComparison.Ordinal);
        Assert.Contains("staleGlobalOnlyRolePermissions", source, StringComparison.Ordinal);
        Assert.Contains("RemoveRange(staleGlobalOnlyRolePermissions)", source, StringComparison.Ordinal);
        Assert.Contains("RoleCodes.SuperAdmin", source, StringComparison.Ordinal);
    }

    private static string ReadSecuritySeedRepositorySource()
    {
        var repositoryRoot = FindRepositoryRoot();

        var sourcePath = Path.Combine(
            repositoryRoot,
            "services",
            "api-dotnet",
            "src",
            "Caritas.Brigadas.Infrastructure",
            "Security",
            "SecuritySeedRepository.cs");

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
