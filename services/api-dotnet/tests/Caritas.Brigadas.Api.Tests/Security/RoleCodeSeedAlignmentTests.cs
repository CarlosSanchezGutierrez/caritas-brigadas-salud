using Caritas.Brigadas.Application.Security;
using Xunit;

namespace Caritas.Brigadas.Api.Tests.Security;

public sealed class RoleCodeSeedAlignmentTests
{
    [Fact]
    public void RoleCodes_All_MatchesSeededRoleModel()
    {
        var expected = new[]
        {
            RoleCodes.SuperAdmin,
            RoleCodes.Admin,
            RoleCodes.BrigadeCoordinator,
            RoleCodes.HealthProvider,
            RoleCodes.ServiceStudent,
            RoleCodes.Auditor,
            RoleCodes.DataAnalyst
        };

        Assert.Equal(
            expected.Order(StringComparer.OrdinalIgnoreCase),
            RoleCodes.All.Order(StringComparer.OrdinalIgnoreCase));
    }

    [Fact]
    public void RoleCodes_All_HasNoDuplicates()
    {
        var unique = RoleCodes.All
            .Distinct(StringComparer.OrdinalIgnoreCase)
            .Count();

        Assert.Equal(RoleCodes.All.Count, unique);
    }

    [Fact]
    public void SecuritySeedRepository_UsesRoleCodesForSeededRoleDefinitionsAndPermissionMap()
    {
        var source = ReadSecuritySeedRepositorySource();

        Assert.Contains("RoleCodes.SuperAdmin", source, StringComparison.Ordinal);
        Assert.Contains("RoleCodes.Admin", source, StringComparison.Ordinal);
        Assert.Contains("RoleCodes.BrigadeCoordinator", source, StringComparison.Ordinal);
        Assert.Contains("RoleCodes.HealthProvider", source, StringComparison.Ordinal);
        Assert.Contains("RoleCodes.ServiceStudent", source, StringComparison.Ordinal);
        Assert.Contains("RoleCodes.Auditor", source, StringComparison.Ordinal);
        Assert.Contains("RoleCodes.DataAnalyst", source, StringComparison.Ordinal);

        Assert.DoesNotContain("[\"SUPER_ADMIN\"]", source, StringComparison.Ordinal);
        Assert.DoesNotContain("[\"ADMIN\"]", source, StringComparison.Ordinal);
        Assert.DoesNotContain("[\"BRIGADE_COORDINATOR\"]", source, StringComparison.Ordinal);
        Assert.DoesNotContain("[\"HEALTH_PROVIDER\"]", source, StringComparison.Ordinal);
        Assert.DoesNotContain("[\"SERVICE_STUDENT\"]", source, StringComparison.Ordinal);
        Assert.DoesNotContain("[\"AUDITOR\"]", source, StringComparison.Ordinal);
        Assert.DoesNotContain("[\"DATA_ANALYST\"]", source, StringComparison.Ordinal);
    }

    [Fact]
    public void RoleCodes_DoesNotExposeDeprecatedUnseededRoleNames()
    {
        Assert.DoesNotContain("ORGANIZATION_ADMIN", RoleCodes.All);
        Assert.DoesNotContain("COORDINATOR", RoleCodes.All);
        Assert.DoesNotContain("RECEPTION", RoleCodes.All);
        Assert.DoesNotContain("VIEWER", RoleCodes.All);
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