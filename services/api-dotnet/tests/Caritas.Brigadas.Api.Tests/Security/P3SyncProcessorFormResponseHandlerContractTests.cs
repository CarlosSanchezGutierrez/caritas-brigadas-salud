using Xunit;

namespace Caritas.Brigadas.Api.Tests.Security;

public sealed class P3SyncProcessorFormResponseHandlerContractTests
{
    [Fact]
    public void SyncBatchProcessor_HandlesFormResponseCreateOnly()
    {
        var source = File.ReadAllText(GetInfrastructurePath("Sync", "SyncBatchProcessor.cs"));

        var requiredTokens = new[]
        {
            "HandleFormResponseEventAsync",
            "syncEvent.EntityType == SyncEntityType.FormResponse",
            "syncEvent.Operation != SyncOperation.Create",
            "form_response_operation_not_implemented",
            "JsonSerializer.Deserialize<CreateFormResponseRequest>",
            "new FormResponse(",
            "_dbContext.FormResponses.Add(formResponse)",
            "syncEvent.Accept(",
            "formResponse.Id",
            "form_response_encounter_not_found",
            "form_response_brigade_mismatch",
            "form_response_template_not_found",
            "form_response_template_inactive",
            "form_response_template_not_yet_effective",
            "form_response_template_expired",
            "form_response_submitted_by_user_not_found",
            "form_response_id_already_exists",
            "form_response_duplicate_encounter_template",
            "form_response_duplicate_encounter_template_in_pending_batch",
            "acceptedFormResponseIdsInBatch",
            "acceptedFormResponseEncounterTemplateKeysInBatch",
            "reserved only after successful FormResponse construction",
            "return 4;",
            "return 5;"
        };

        AssertRequiredTokens(source, requiredTokens, "SyncBatchProcessor form response handler");

        var forbiddenTokens = System.Array.Empty<string>();

        foreach (var token in forbiddenTokens)
        {
            Assert.DoesNotContain(token, source, StringComparison.Ordinal);
        }
    }

    [Fact]
    public void FormResponseHandlerBaseline_DefinesFormResponseOnlyScope()
    {
        var source = File.ReadAllText(GetDocPath("P3_SYNC_PROCESSOR_FORM_RESPONSE_HANDLER_BASELINE.md"));

        var requiredTokens = new[]
        {
            "P3 Sync Processor Form Response Handler Baseline",
            "EntityType: form_response",
            "Operation: create",
            "parse PayloadJson as CreateFormResponseRequest",
            "validate ResponseJson is valid JSON",
            "processor must process service_encounter create events before form_response create events",
            "processor must not log raw PayloadJson or ResponseJson",
            "form_response update is not implemented in P3-17",
            "form_response void is not implemented in P3-17",
            "Acceptance criteria"
        };

        AssertRequiredTokens(source, requiredTokens, "P3 sync processor form response handler baseline");
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