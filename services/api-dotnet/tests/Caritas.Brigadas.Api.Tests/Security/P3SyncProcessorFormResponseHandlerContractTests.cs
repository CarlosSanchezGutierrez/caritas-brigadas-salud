using Xunit;

namespace Caritas.Brigadas.Api.Tests.Security;

public sealed class P3SyncProcessorFormResponseHandlerContractTests
{
    [Fact]
    public void SyncBatchProcessor_HandlesFormResponseCreateOnly()
    {
        var source =
            File.ReadAllText(GetInfrastructurePath("Sync", "SyncBatchProcessor.cs")) +
            File.ReadAllText(GetInfrastructurePath("Sync", "SyncProcessingOrder.cs")) +
            File.ReadAllText(GetInfrastructurePath("Sync", "FormResponseSyncEventHandler.cs"));

        var requiredTokens = new[]
        {
            "await _formResponseSyncEventHandler.HandleAsync(",
            "FormResponseSyncEventHandler",
            "SyncEntityType.FormResponse",
            "out CreateFormResponseRequest? request",
            "JsonDocument.Parse(request.ResponseJson)",
            "var formResponse = new FormResponse(",
            "_dbContext.FormResponses.Add(formResponse)",
            "syncEvent.Accept(",
            "formResponse.Id",
            "form_response_operation_not_implemented",
            "form_response_encounter_not_found",
            "form_response_brigade_mismatch",
            "form_response_template_not_found",
            "form_response_template_inactive",
            "form_response_template_not_yet_effective",
            "form_response_template_expired",
            "form_response_submitted_by_user_not_found",
            "form_response_id_already_exists",
            "form_response_duplicate_in_pending_batch",
            "form_response_duplicate_encounter_template_in_pending_batch",
            "form_response_duplicate_encounter_template",
            "acceptedFormResponseIdsInBatch",
            "acceptedFormResponseEncounterTemplateKeysInBatch",
            "reserved only after successful FormResponse construction and reserved atomically",
            "formResponseIdReserved",
            "formResponseEncounterTemplateKeyReserved",
            "acceptedFormResponseIdsInBatch.Remove(formResponseId)"
        };

        AssertRequiredTokens(source, requiredTokens, "SyncBatchProcessor form response handler");
    }

    [Fact]
    public void SyncBatchProcessor_DoesNotContainDirectFormResponseLogicAfterExtraction()
    {
        var source = File.ReadAllText(GetInfrastructurePath("Sync", "SyncBatchProcessor.cs"));

        var requiredTokens = new[]
        {
            "private readonly FormResponseSyncEventHandler _formResponseSyncEventHandler;",
            "_formResponseSyncEventHandler = new FormResponseSyncEventHandler(dbContext, PayloadJsonOptions);",
            "await _formResponseSyncEventHandler.HandleAsync(",
            "await _formResponseSyncEventHandler.HandleAsync("
        };

        AssertRequiredTokens(source, requiredTokens, "SyncBatchProcessor form response wrapper");

        var forbiddenTokens = new[]
        {
            "out CreateFormResponseRequest? request",
            "var formResponse = new FormResponse(",
            "_dbContext.FormResponses.Add(formResponse)",
            "form_response_operation_not_implemented",
            "form_response_encounter_not_found",
            "form_response_brigade_mismatch",
            "form_response_template_not_found",
            "form_response_template_inactive",
            "form_response_template_not_yet_effective",
            "form_response_template_expired",
            "form_response_submitted_by_user_not_found",
            "form_response_id_already_exists",
            "form_response_duplicate_in_pending_batch",
            "form_response_duplicate_encounter_template_in_pending_batch",
            "form_response_duplicate_encounter_template",
            "formResponseIdReserved",
            "formResponseEncounterTemplateKeyReserved",
            "acceptedFormResponseIdsInBatch.Remove(formResponseId)"
        };

        foreach (var token in forbiddenTokens)
        {
            Assert.DoesNotContain(token, source, StringComparison.Ordinal);
        }
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
