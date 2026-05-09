using Xunit;

namespace Caritas.Brigadas.Api.Tests.Audit;

public sealed class AuditLogsPaginationContractTests
{
    [Fact]
    public void AuditLogsController_ListEndpoint_UsesPaginatedResponse()
    {
        var source = ReadRepoFile(
            "src",
            "Caritas.Brigadas.Api",
            "Controllers",
            "AuditLogsController.cs");

        Assert.Contains("ApiResponse<PaginatedResponse<AuditLogSummaryDto>>", source);
        Assert.Contains("[FromQuery] PaginationRequest pagination", source);
        Assert.DoesNotContain("ApiResponse<IReadOnlyCollection<AuditLogSummaryDto>>", source);
    }

    [Fact]
    public void AuditLogReadRepository_ListEndpoint_UsesCountSkipAndTake()
    {
        var source = ReadRepoFile(
            "src",
            "Caritas.Brigadas.Infrastructure",
            "Audit",
            "AuditLogReadRepository.cs");

        Assert.Contains("CountAsync(cancellationToken)", source);
        Assert.Contains(".Skip(pagination.Skip)", source);
        Assert.Contains(".Take(pageSize)", source);
        Assert.Contains("ThenByDescending(auditLog => auditLog.Id)", source);
        Assert.DoesNotContain(".Take(250)", source);
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