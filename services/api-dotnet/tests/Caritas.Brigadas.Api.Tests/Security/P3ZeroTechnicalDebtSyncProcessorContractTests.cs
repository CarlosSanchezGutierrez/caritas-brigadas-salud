using System.Text.RegularExpressions;
using Xunit;

namespace Caritas.Brigadas.Api.Tests.Security;

public sealed class P3ZeroTechnicalDebtSyncProcessorContractTests
{
    [Fact]
    public void SyncBatchProcessor_DoesNotAllowNewDirectHandlersBeforeDecomposition()
    {
        var source = File.ReadAllText(GetInfrastructurePath("Sync", "SyncBatchProcessor.cs"));

        var handlerCount = Regex.Matches(
            source,
            "private async Task Handle[A-Za-z]+EventAsync").Count;

        Assert.True(
            handlerCount <= 8,
            $"SyncBatchProcessor has {handlerCount} handlers. No new handlers are allowed before decomposition.");
    }

    [Fact]
    public void SyncBatchProcessor_DoesNotContainZeroDebtForbiddenPatterns()
    {
        var source = File.ReadAllText(GetInfrastructurePath("Sync", "SyncBatchProcessor.cs"));

        var forbiddenPatterns = new[]
        {
            @"(?m)[ \t]+$",
            @"(?m)^ {20,}// Pending-batch",
            "TODO",
            "HACK",
            "quick fix",
            "temporary workaround",
            "technical debt accepted"
        };

        var failures = forbiddenPatterns
            .Where(pattern => Regex.IsMatch(source, pattern, RegexOptions.IgnoreCase))
            .Select(pattern => $"Forbidden zero-debt pattern found: {pattern}")
            .ToArray();

        Assert.True(
            failures.Length == 0,
            "SyncBatchProcessor contains zero-debt violations." +
            Environment.NewLine +
            string.Join(Environment.NewLine, failures));
    }

    [Fact]
    public void ZeroTechnicalDebtBaseline_DefinesMandatoryDecompositionPath()
    {
        var source = File.ReadAllText(GetDocPath("P3_ZERO_TECHNICAL_DEBT_SYNC_PROCESSOR_BASELINE.md"));

        var requiredTokens = new[]
        {
            "P3 Zero Technical Debt Sync Processor Baseline",
            "This baseline does not permit technical debt",
            "no new sync entity handlers may be added directly to SyncBatchProcessor before decomposition",
            "extract sync processing order into a dedicated internal component",
            "extract pending-batch reservation state into a dedicated internal component",
            "extract payload parsing/validation into a dedicated internal component",
            "extract each domain handler into a dedicated internal handler class",
            "Acceptance criteria"
        };

        AssertRequiredTokens(source, requiredTokens, "P3 zero technical debt sync processor baseline");
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
