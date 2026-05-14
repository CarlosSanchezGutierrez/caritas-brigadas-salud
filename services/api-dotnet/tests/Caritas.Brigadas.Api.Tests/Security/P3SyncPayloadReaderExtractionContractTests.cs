using Xunit;

namespace Caritas.Brigadas.Api.Tests.Security;

public sealed class P3SyncPayloadReaderExtractionContractTests
{
    [Fact]
    public void SyncPayloadReader_OwnsJsonObjectParsingAndDeserialization()
    {
        var source = File.ReadAllText(GetInfrastructurePath("Sync", "SyncPayloadReader.cs"));

        var requiredTokens = new[]
        {
            "internal static class SyncPayloadReader",
            "public static bool TryReadObject<TRequest>",
            "[NotNullWhen(true)] out TRequest? request",
            "where TRequest : class",
            "JsonDocument.Parse(payloadJson)",
            "document.RootElement.ValueKind != JsonValueKind.Object",
            "document.RootElement.Deserialize<TRequest>(serializerOptions)",
            "payload must be a JSON object.",
            "payload JSON is invalid.",
            "payload is required."
        };

        AssertRequiredTokens(source, requiredTokens, "SyncPayloadReader");
    }

    [Fact]
    public void SyncBatchProcessor_UsesSyncPayloadReaderForCurrentCreateDtos()
    {
        var source = File.ReadAllText(GetInfrastructurePath("Sync", "SyncBatchProcessor.cs")) + File.ReadAllText(GetInfrastructurePath("Sync", "PatientSyncEventHandler.cs")) + File.ReadAllText(GetInfrastructurePath("Sync", "PatientVisitSyncEventHandler.cs")) + File.ReadAllText(GetInfrastructurePath("Sync", "ServiceEncounterSyncEventHandler.cs")) + File.ReadAllText(GetInfrastructurePath("Sync", "VitalSignsSyncEventHandler.cs"));

        var requiredTokens = new[]
        {
            "SyncPayloadReader.TryReadObject",
            "out CreatePatientRequest? request",
            "out CreatePatientVisitRequest? request",
            "out CreateServiceEncounterRequest? request",
            "out CreateVitalSignsRecordRequest? request",
            "out CreateFormResponseRequest? request",
            "out CreateConsentDocumentRequest? request",
            "out CreateMedicalReferralRequest? request",
            "out CreateMedicationDeliveryRequest? request",
            "payloadRejectionReason"
        };

        AssertRequiredTokens(source, requiredTokens, "SyncBatchProcessor payload reader extraction");

        var forbiddenTokens = new[]
        {
            "JsonSerializer.Deserialize<CreatePatientRequest>",
            "JsonSerializer.Deserialize<CreatePatientVisitRequest>",
            "JsonSerializer.Deserialize<CreateServiceEncounterRequest>",
            "JsonSerializer.Deserialize<CreateVitalSignsRecordRequest>",
            "JsonSerializer.Deserialize<CreateFormResponseRequest>",
            "JsonSerializer.Deserialize<CreateConsentDocumentRequest>",
            "JsonSerializer.Deserialize<CreateMedicalReferralRequest>",
            "JsonSerializer.Deserialize<CreateMedicationDeliveryRequest>",
            "out  request"
        };

        foreach (var token in forbiddenTokens)
        {
            Assert.DoesNotContain(token, source, StringComparison.Ordinal);
        }
    }

    [Fact]
    public void PayloadReaderExtractionBaseline_DefinesCentralizedPayloadParsing()
    {
        var source = File.ReadAllText(GetDocPath("P3_SYNC_PAYLOAD_READER_EXTRACTION_BASELINE.md"));

        var requiredTokens = new[]
        {
            "P3 Sync Payload Reader Extraction Baseline",
            "SyncPayloadReader",
            "parse PayloadJson",
            "require JSON object root",
            "deserialize the request DTO",
            "SyncBatchProcessor must use SyncPayloadReader.TryReadObject for all current create request DTOs",
            "SyncBatchProcessor must use explicit typed out variables for current create request DTOs",
            "Acceptance criteria"
        };

        AssertRequiredTokens(source, requiredTokens, "P3 sync payload reader extraction baseline");
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
