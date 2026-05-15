using System.Text.RegularExpressions;
using Xunit;

namespace Caritas.Brigadas.Api.Tests.Security;

public sealed class P3PreMainSyncCodexReviewFindingsTests
{
    [Fact]
    public void PatientSyncEventHandler_TreatsSoftDeletedPatientIdsAsConflicts()
    {
        var source = File.ReadAllText(GetInfrastructureSourcePath("Sync", "PatientSyncEventHandler.cs"));

        Assert.Contains("patient_id_already_exists", source, StringComparison.Ordinal);

        Assert.DoesNotMatch(
            @"patient\.Id == patientId &&\s*patient\.OrganizationId == organizationId &&\s*!patient\.IsDeleted",
            source);
    }

    [Fact]
    public void SyncBatchWriteRepository_HashesLongServerGeneratedIdempotencyKeys()
    {
        var source = File.ReadAllText(GetInfrastructureSourcePath("Sync", "SyncBatchWriteRepository.cs"));

        Assert.Contains("MaxIdempotencyKeyLength = 250", source, StringComparison.Ordinal);
        Assert.Contains("BuildHashedIdempotencyKey", source, StringComparison.Ordinal);
        Assert.Contains("SHA256.HashData", source, StringComparison.Ordinal);
        Assert.Contains("Encoding.UTF8.GetBytes(rawKey)", source, StringComparison.Ordinal);
        Assert.Contains("EnsureIdempotencyKeyLength", source, StringComparison.Ordinal);

        Assert.DoesNotContain(
            "return $\"org:{organizationId:N}:user:{userId:N}:brigade:{brigadeId:N}:client:{clientInstanceId}:event:{localEventId}\";",
            source,
            StringComparison.Ordinal);
    }

    private static string GetInfrastructureSourcePath(params string[] parts)
    {
        return Path.Combine(
            new[]
            {
                FindRepositoryRoot(),
                "services",
                "api-dotnet",
                "src",
                "Caritas.Brigadas.Infrastructure"
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
