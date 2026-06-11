using Xunit;

namespace Caritas.Brigadas.Api.Tests.Security;

public sealed class P5PatientModuleClosureContractTests
{
    [Fact]
    public void PatientModuleClosure_DocumentsControlledMilestoneWithoutProductionReadinessClaim()
    {
        var source = File.ReadAllText(GetRepositoryPath("docs", "implementation", "P5_10_PATIENT_MODULE_CLOSURE.md"));

        var requiredTokens = new[]
        {
            "P5.10 Patient Module Closure",
            "Patient module backend controlled milestone: CLOSED_PENDING_REAL_ENVIRONMENT_EVIDENCE",
            "Backend production readiness: BLOCKED_PENDING_REAL_EVIDENCE",
            "No backend production readiness approval",
            "No fabricated evidence",
            "No secrets in repository",
            "No committed real patient data",
            "No direct mobile write to SQL Server",
            "No client may bypass the API",
            "No cloud dependency",
            "SQL Server remains the operational source of truth"
        };

        AssertRequiredTokens(source, requiredTokens, "patient module closure documentation");

        Assert.DoesNotContain("Backend production readiness: APPROVED", source, StringComparison.OrdinalIgnoreCase);
        Assert.DoesNotContain("Patient module backend controlled milestone: PRODUCTION_READY", source, StringComparison.OrdinalIgnoreCase);
    }

    [Fact]
    public void PatientModuleClosure_VerifierExistsAndProtectsCriticalEvidence()
    {
        var source = File.ReadAllText(GetRepositoryPath("scripts", "verify-p5-10-patient-module-closure.ps1"));

        var requiredTokens = new[]
        {
            "verify-p5-03",
            "verify-p5-04",
            "verify-p5-05",
            "verify-p5-06",
            "verify-p5-07",
            "verify-p5-08",
            "verify-p5-09",
            "FindExistingIdempotentPatientAsync",
            "PatientClinicalRecordTimelineEventDto",
            "AuditActionCodes.PatientCreate",
            "Backend production readiness: BLOCKED_PENDING_REAL_EVIDENCE"
        };

        AssertRequiredTokens(source, requiredTokens, "patient module closure verifier");
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
            $"{label} contract is incomplete." +
            Environment.NewLine +
            string.Join(Environment.NewLine, failures));
    }

    private static string GetRepositoryPath(params string[] segments)
    {
        return Path.Combine(new[] { FindRepositoryRoot() }.Concat(segments).ToArray());
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