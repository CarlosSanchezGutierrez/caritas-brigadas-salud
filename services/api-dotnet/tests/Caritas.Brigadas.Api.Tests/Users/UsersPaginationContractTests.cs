using Xunit;

namespace Caritas.Brigadas.Api.Tests.Users;

public sealed class UsersPaginationContractTests
{
    [Fact]
    public void UsersController_ListEndpoint_UsesPaginatedResponse()
    {
        var source = ReadRepoFile(
            "src",
            "Caritas.Brigadas.Api",
            "Controllers",
            "UsersController.cs");

        Assert.Contains("ApiResponse<PaginatedResponse<UserSummaryDto>>", source);
        Assert.Contains("[FromQuery] PaginationRequest pagination", source);
        Assert.DoesNotContain("ApiResponse<IReadOnlyCollection<UserSummaryDto>>", source);
    }

    [Fact]
    public void UserReadRepository_ListEndpoint_UsesCountSkipAndTake()
    {
        var source = ReadRepoFile(
            "src",
            "Caritas.Brigadas.Infrastructure",
            "Users",
            "UserReadRepository.cs");

        Assert.Contains("CountAsync(cancellationToken)", source);
        Assert.Contains(".Skip(pagination.Skip)", source);
        Assert.Contains(".Take(pageSize)", source);
        Assert.Contains("ThenBy(user => user.Id)", source);
        Assert.DoesNotContain(".Take(250)", source);
    }

    [Fact]
    public void UserReadRepositoryContract_ReturnsPaginatedResponse()
    {
        var source = ReadRepoFile(
            "src",
            "Caritas.Brigadas.Application",
            "Users",
            "IUserReadRepository.cs");

        Assert.Contains("Task<PaginatedResponse<UserSummaryDto>> ListByOrganizationAsync", source);
        Assert.Contains("PaginationRequest pagination", source);
    }

    private static string ReadRepoFile(params string[] relativeSegments)
    {
        var current = new DirectoryInfo(AppContext.BaseDirectory);

        while (current is not null)
        {
            var candidate = Path.Combine(
                new[] { current.FullName }.Concat(relativeSegments).ToArray());

            if (File.Exists(candidate))
            {
                return File.ReadAllText(candidate);
            }

            current = current.Parent;
        }

        throw new FileNotFoundException(
            $"Could not find repository file: {Path.Combine(relativeSegments)}");
    }
}