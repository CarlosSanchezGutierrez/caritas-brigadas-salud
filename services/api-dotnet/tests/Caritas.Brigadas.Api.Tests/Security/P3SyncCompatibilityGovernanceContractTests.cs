using Xunit;

namespace Caritas.Brigadas.Api.Tests.Security;

public sealed class P3SyncCompatibilityGovernanceContractTests
{
    [Fact]
    public void CompatibilityGovernanceBaseline_DefinesZeroDebtInterpretation()
    {
        var source = File.ReadAllText(GetDocPath("P3_SYNC_COMPATIBILITY_GOVERNANCE_BASELINE.md"));

        var requiredTokens = new[]
        {
            "P3 Sync Compatibility Governance Baseline",
            "compatibility governance",
            "not accepted technical debt",
            "Compatibility governance means",
            "Zero technical debt interpretation",
            "Backend closure path",
            "PatientSyncEventHandler",
            "PatientVisitSyncEventHandler",
            "ServiceEncounterSyncEventHandler",
            "MedicationDeliverySyncEventHandler"
        };

        AssertRequiredTokens(source, requiredTokens, "P3 sync compatibility governance baseline");
    }

    [Fact]
    public void ActiveP3SyncGovernance_DoesNotUseLegacyTerminology()
    {
        var root = FindRepositoryRoot();

        var roots = new[]
        {
            Path.Combine(root, "docs", "backend"),
            Path.Combine(root, "scripts"),
            Path.Combine(root, "services", "api-dotnet", "tests", "Caritas.Brigadas.Api.Tests", "Security")
        };

        var offenders = roots
            .Where(Directory.Exists)
            .SelectMany(path => Directory.GetFiles(path, "*.*", SearchOption.AllDirectories))
            .Where(path => path.EndsWith(".md", StringComparison.OrdinalIgnoreCase) ||
                           path.EndsWith(".ps1", StringComparison.OrdinalIgnoreCase) ||
                           path.EndsWith(".cs", StringComparison.OrdinalIgnoreCase))
            .SelectMany(path => File.ReadAllLines(path)
                .Select((line, index) => new { path, line, number = index + 1 }))
            .Where(item => item.line.Contains("legacy", StringComparison.OrdinalIgnoreCase))
            .Select(item => $"{item.path}:{item.number}:{item.line}")
            .ToArray();

        Assert.True(
            offenders.Length == 0,
            "Active P3 sync governance must use compatibility terminology instead of legacy terminology." +
            Environment.NewLine +
            string.Join(Environment.NewLine, offenders));
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