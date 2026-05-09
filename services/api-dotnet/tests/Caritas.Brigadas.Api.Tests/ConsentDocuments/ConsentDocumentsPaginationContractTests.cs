using Xunit;

namespace Caritas.Brigadas.Api.Tests.ConsentDocuments;

public sealed class ConsentDocumentsPaginationContractTests
{
    [Fact]
    public void ConsentDocumentsController_ListEndpoint_UsesPaginatedResponse()
    {
        var source = ReadRepoFile(
            "src",
            "Caritas.Brigadas.Api",
            "Controllers",
            "ConsentDocumentsController.cs");

        Assert.Contains("ApiResponse<PaginatedResponse<ConsentDocumentSummaryDto>>", source);
        Assert.Contains("[FromQuery] PaginationRequest pagination", source);
        Assert.DoesNotContain("ApiResponse<IReadOnlyCollection<ConsentDocumentSummaryDto>>", source);
    }

    [Fact]
    public void ConsentDocumentReadRepository_ListEndpoint_UsesCountSkipAndTake()
    {
        var source = ReadRepoFile(
            "src",
            "Caritas.Brigadas.Infrastructure",
            "ConsentDocuments",
            "ConsentDocumentReadRepository.cs");

        Assert.Contains("CountAsync(cancellationToken)", source);
        Assert.Contains(".Skip(pagination.Skip)", source);
        Assert.Contains(".Take(pageSize)", source);
        Assert.Contains(".OrderByDescending(document => EF.Property<Guid>(document, \"Id\"))", source);
        Assert.Contains("Select(MapToDto)", source);
        Assert.DoesNotContain(".Take(250)", source);
    }

    [Fact]
    public void ConsentDocumentReadRepositoryContract_ReturnsPaginatedResponse()
    {
        var source = ReadRepoFile(
            "src",
            "Caritas.Brigadas.Application",
            "ConsentDocuments",
            "IConsentDocumentReadRepository.cs");

        Assert.Contains("Task<PaginatedResponse<ConsentDocumentSummaryDto>> ListByOrganizationAsync", source);
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