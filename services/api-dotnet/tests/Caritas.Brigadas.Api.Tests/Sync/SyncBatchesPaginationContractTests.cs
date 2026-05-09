using Xunit;

namespace Caritas.Brigadas.Api.Tests.Sync;

public sealed class SyncBatchesPaginationContractTests
{
    [Fact]
    public void SyncBatchesController_ListEndpoint_UsesPaginatedResponse()
    {
        var source = ReadRepoFile(
            "src",
            "Caritas.Brigadas.Api",
            "Controllers",
            "SyncBatchesController.cs");

        Assert.Contains("ApiResponse<PaginatedResponse<SyncBatchSummaryDto>>", source);
        Assert.Contains("[FromQuery] PaginationRequest pagination", source);
        Assert.DoesNotContain("ApiResponse<IReadOnlyCollection<SyncBatchSummaryDto>>", source);
    }

    [Fact]
    public void SyncBatchReadRepository_ListEndpoint_UsesCountSkipAndTake()
    {
        var source = ReadRepoFile(
            "src",
            "Caritas.Brigadas.Infrastructure",
            "Sync",
            "SyncBatchReadRepository.cs");

        Assert.Contains("CountAsync(cancellationToken)", source);
        Assert.Contains(".Skip(pagination.Skip)", source);
        Assert.Contains(".Take(pageSize)", source);
        Assert.Contains("ThenByDescending(batch => batch.Id)", source);
        Assert.DoesNotContain(".Take(250)", source);
    }

    [Fact]
    public void SyncBatchReadRepositoryContract_ReturnsPaginatedResponse()
    {
        var source = ReadRepoFile(
            "src",
            "Caritas.Brigadas.Application",
            "Sync",
            "ISyncBatchReadRepository.cs");

        Assert.Contains("Task<PaginatedResponse<SyncBatchSummaryDto>> ListByOrganizationAsync", source);
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