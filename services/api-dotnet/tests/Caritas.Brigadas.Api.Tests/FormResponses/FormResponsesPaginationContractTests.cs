using Xunit;

namespace Caritas.Brigadas.Api.Tests.FormResponses;

public sealed class FormResponsesPaginationContractTests
{
    [Fact]
    public void FormResponsesController_ListEndpoint_UsesPaginatedResponse()
    {
        var source = ReadRepoFile(
            "src",
            "Caritas.Brigadas.Api",
            "Controllers",
            "FormResponsesController.cs");

        Assert.Contains("ApiResponse<PaginatedResponse<FormResponseSummaryDto>>", source);
        Assert.Contains("[FromQuery] PaginationRequest pagination", source);
        Assert.DoesNotContain("ApiResponse<IReadOnlyCollection<FormResponseSummaryDto>>", source);
    }

    [Fact]
    public void FormResponseReadRepository_ListEndpoint_UsesCountSkipAndTake()
    {
        var source = ReadRepoFile(
            "src",
            "Caritas.Brigadas.Infrastructure",
            "FormResponses",
            "FormResponseReadRepository.cs");

        Assert.Contains("CountAsync(cancellationToken)", source);
        Assert.Contains(".Skip(pagination.Skip)", source);
        Assert.Contains(".Take(pageSize)", source);
        Assert.Contains(".OrderByDescending(response => EF.Property<Guid>(response, \"Id\"))", source);
        Assert.Contains("Select(MapToDto)", source);
        Assert.DoesNotContain(".Take(250)", source);
    }

    [Fact]
    public void FormResponseReadRepositoryContract_ReturnsPaginatedResponse()
    {
        var source = ReadRepoFile(
            "src",
            "Caritas.Brigadas.Application",
            "FormResponses",
            "IFormResponseReadRepository.cs");

        Assert.Contains("Task<PaginatedResponse<FormResponseSummaryDto>> ListByOrganizationAsync", source);
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