using Xunit;

namespace Caritas.Brigadas.Api.Tests.Security;

public sealed class P3PreMainSyncCodexReviewFindingsTests
{
    [Fact]
    public void PatientSyncEventHandler_TreatsSoftDeletedPatientIdsAsConflicts()
    {
        var source = File.ReadAllText(GetInfrastructureSourcePath("Sync", "PatientSyncEventHandler.cs"));

        var startToken = "        var patientIdAlreadyExists = await _dbContext.Patients";
        var endToken = "        if (patientIdAlreadyExists ||";

        var startIndex = source.IndexOf(startToken, StringComparison.Ordinal);
        var endIndex = source.IndexOf(endToken, startIndex, StringComparison.Ordinal);

        Assert.True(startIndex >= 0, "patientIdAlreadyExists block must exist.");
        Assert.True(endIndex > startIndex, "patientIdAlreadyExists block must end before conflict check.");

        var patientIdAlreadyExistsBlock = source[startIndex..endIndex];

        Assert.Contains("patient_id_already_exists", source, StringComparison.Ordinal);

        Assert.Matches(
            @"patient\.Id == patientId\s*&&\s*patient\.OrganizationId == organizationId\s*,",
            patientIdAlreadyExistsBlock);

        Assert.DoesNotContain("!patient.IsDeleted", patientIdAlreadyExistsBlock, StringComparison.Ordinal);

        Assert.Contains(
            "_dbContext.Patients.Local.Any(patient => patient.Id == patientId && patient.OrganizationId == organizationId)",
            source,
            StringComparison.Ordinal);

        Assert.DoesNotContain(
            "_dbContext.Patients.Local.Any(patient => patient.Id == patientId && patient.OrganizationId == organizationId && !patient.IsDeleted)",
            source,
            StringComparison.Ordinal);
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


    [Fact]
    public void SyncBatchWriteRepository_DoesNotPersistEmptyRetryBatches()
    {
        var source = File.ReadAllText(GetInfrastructureSourcePath("Sync", "SyncBatchWriteRepository.cs"));

        Assert.Contains("existingEvents.Length > 0", source, StringComparison.Ordinal);
        Assert.Contains("return ToSummaryDto(existingBatch);", source, StringComparison.Ordinal);
        Assert.Contains("Payload contains sync events that were already submitted in a different batch.", source, StringComparison.Ordinal);
        Assert.Contains("_dbContext.SyncBatches.Add(batch);", source, StringComparison.Ordinal);
        Assert.Contains("_dbContext.SyncEvents.AddRange(events);", source, StringComparison.Ordinal);

        Assert.DoesNotContain("var newEvents = events", source, StringComparison.Ordinal);
        Assert.DoesNotContain("_dbContext.SyncEvents.AddRange(newEvents);", source, StringComparison.Ordinal);
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
