using Xunit;

namespace Caritas.Brigadas.Api.Tests.Security;

public sealed class P3SyncProcessorPatientHandlerContractTests
{
    [Fact]
    public void SyncBatchProcessor_HandlesPatientCreateOnly()
    {
        var source = File.ReadAllText(GetInfrastructurePath("Sync", "SyncBatchProcessor.cs")) + File.ReadAllText(GetInfrastructurePath("Sync", "PatientSyncEventHandler.cs"));

        var requiredTokens = new[]
        {
            "await _patientSyncEventHandler.HandleAsync(",
            "syncEvent.EntityType == SyncEntityType.Patient",
            "syncEvent.Operation != SyncOperation.Create",
            "patient_operation_not_implemented",
            "out CreatePatientRequest? request",
            "new Patient(",
            "_dbContext.Patients.Add(patient)",
            "syncEvent.Accept(",
            "patient.Id",
            "patient_folio_already_exists",
            "patient_folio_duplicate_in_pending_batch",
            "acceptedPatientFoliosInBatch",
            "acceptedPatientFoliosInBatch.Contains(normalizedFolio)",
            "!acceptedPatientFoliosInBatch.Add(normalizedFolio)",
            "GenerateSyncPatientFolio",
            "ParseSex"
        };

        AssertRequiredTokens(source, requiredTokens, "SyncBatchProcessor patient handler");

        var forbiddenTokens = System.Array.Empty<string>();

        foreach (var token in forbiddenTokens)
        {
            Assert.DoesNotContain(token, source, StringComparison.Ordinal);
        }
    }

    [Fact]
    public void PatientHandlerBaseline_DefinesPatientOnlyScope()
    {
        var source = File.ReadAllText(GetDocPath("P3_SYNC_PROCESSOR_PATIENT_HANDLER_BASELINE.md"));

        var requiredTokens = new[]
        {
            "P3 Sync Processor Patient Handler Baseline",
            "EntityType: patient",
            "Operation: create",
            "parse PayloadJson as CreatePatientRequest",
            "create Patient with OrganizationId from the sync batch route/context",
            "conflict duplicate PatientFolio inside the organization",
            "duplicate PatientFolio values inside the same pending batch",
            "set SyncEvent.EntityId to the created Patient.Id",
            "patient update is not implemented in P3-13",
            "patient void is not implemented in P3-13",
            "processor must not create visits, encounters, vital signs, forms, documents, referrals, or medication deliveries in P3-13",
            "Acceptance criteria",
            "P3-14 patient visit handler note",
            "P3-15 vital signs handler note",
            "P3-16 service encounter handler note",
            "P3-17 form response handler note",
            "P3-18 consent document handler note"
        };

        AssertRequiredTokens(source, requiredTokens, "P3 sync processor patient handler baseline");
    }

    private static void AssertRequiredTokens(
        string source,
        IReadOnlyCollection<string> requiredTokens,
        string label)
    {
        var failures = requiredTokens
            .Where(token => !source.Contains(token, StringComparison.Ordinal))
            .Select(token => $"{label} is missing required token: {token}")
            .ToArray();

        Assert.True(
            failures.Length == 0,
            $"{label} is incomplete." +
            Environment.NewLine +
            string.Join(Environment.NewLine, failures));
    }

    private static string GetInfrastructurePath(params string[] segments)
    {
        return Path.Combine(
            new[] { FindRepositoryRoot(), "services", "api-dotnet", "src", "Caritas.Brigadas.Infrastructure" }
                .Concat(segments)
                .ToArray());
    }

    private static string GetDocPath(string fileName)
    {
        return Path.Combine(
            FindRepositoryRoot(),
            "docs",
            "backend",
            fileName);
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