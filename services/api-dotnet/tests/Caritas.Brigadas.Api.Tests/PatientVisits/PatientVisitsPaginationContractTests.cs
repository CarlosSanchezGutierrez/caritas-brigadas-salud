using Xunit;

namespace Caritas.Brigadas.Api.Tests.PatientVisits;

public sealed class PatientVisitsPaginationContractTests
{
    [Fact]
    public void PatientVisitsController_ListEndpoint_UsesPaginatedResponse()
    {
        var source = ReadRepoFile(
            "src",
            "Caritas.Brigadas.Api",
            "Controllers",
            "PatientVisitsController.cs");

        Assert.Contains("ApiResponse<PaginatedResponse<PatientVisitSummaryDto>>", source);
        Assert.Contains("[FromQuery] PaginationRequest pagination", source);
        Assert.DoesNotContain("ApiResponse<IReadOnlyCollection<PatientVisitSummaryDto>>", source);
    }

    [Fact]
    public void PatientVisitReadRepository_ListEndpoint_UsesCountSkipAndTake()
    {
        var source = ReadRepoFile(
            "src",
            "Caritas.Brigadas.Infrastructure",
            "PatientVisits",
            "PatientVisitReadRepository.cs");

        Assert.Contains("CountAsync(cancellationToken)", source);
        Assert.Contains(".Skip(pagination.Skip)", source);
        Assert.Contains(".Take(pageSize)", source);
        Assert.Contains("EF.Property<DateTimeOffset?>(visit, \"ArrivalTime\")", source);
        Assert.Contains(".ThenByDescending(visit => EF.Property<Guid>(visit, \"Id\"))", source);
        Assert.Contains("Select(MapToDto)", source);
        Assert.DoesNotContain(".Take(250)", source);
    }

    [Fact]
    public void PatientVisitReadRepositoryContract_ReturnsPaginatedResponse()
    {
        var source = ReadRepoFile(
            "src",
            "Caritas.Brigadas.Application",
            "PatientVisits",
            "IPatientVisitReadRepository.cs");

        Assert.Contains("Task<PaginatedResponse<PatientVisitSummaryDto>> ListByOrganizationAsync", source);
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