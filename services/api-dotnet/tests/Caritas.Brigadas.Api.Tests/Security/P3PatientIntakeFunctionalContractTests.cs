using Xunit;

namespace Caritas.Brigadas.Api.Tests.Security;

public sealed class P3PatientIntakeFunctionalContractTests
{
    [Fact]
    public void PatientIntakeFunctionalContractBaseline_DefinesPatientIntakeScope()
    {
        var source = File.ReadAllText(GetProductDocPath("P3_PATIENT_INTAKE_FUNCTIONAL_CONTRACT_BASELINE.md"));

        var requiredTokens = new[]
        {
            "P3 Patient Intake Functional Contract Baseline",
            "Patient intake must allow incomplete information.",
            "patientId",
            "organizationId",
            "localPatientKey",
            "firstName",
            "paternalLastName",
            "maternalLastName",
            "displayName",
            "dateOfBirth",
            "approximateAgeYears",
            "phoneNumber",
            "isIdentityIncomplete",
            "identityIncompleteReason",
            "capturedAtUtc",
            "capturedByUserId",
            "Migrant or incomplete patient data handling",
            "Social security / insurance fields are finalized in P3-30C.",
            "Consent and signature evidence are finalized in P3-30B.",
            "Acceptance criteria"
        };

        AssertRequiredTokens(source, requiredTokens, "P3 patient intake functional contract baseline");
    }

    [Fact]
    public void PatientIntakeFunctionalContract_DefinesFrontendAndSyncBehavior()
    {
        var source = File.ReadAllText(GetProductDocPath("P3_PATIENT_INTAKE_FUNCTIONAL_CONTRACT.md"));

        var requiredTokens = new[]
        {
            "P3 Patient Intake Functional Contract",
            "Frontend readiness impact: BLOCKS_FULL_FRONTEND",
            "Minimum valid patient intake",
            "Incomplete identity behavior",
            "Validation rules",
            "Spanish frontend labels",
            "Offline sync contract",
            "Search/display behavior",
            "Privacy and logging requirements",
            "patient_created",
            "patient_updated",
            "patient_identity_label_missing",
            "patient_identity_incomplete_reason_missing",
            "MIGRANT_OR_TRANSIENT",
            "NO_DOCUMENTS_AVAILABLE",
            "Nombre",
            "Apellido paterno",
            "Datos incompletos",
            "PARTIAL_FRONTEND_READY"
        };

        AssertRequiredTokens(source, requiredTokens, "P3 patient intake functional contract");
    }

    [Fact]
    public void PatientIntakeFunctionalContractVerifier_RequiresGapAuditAndGovernanceReferences()
    {
        var source = File.ReadAllText(GetScriptPath("verify-p3-patient-intake-functional-contract.ps1"));

        var requiredTokens = new[]
        {
            "P3 patient intake functional contract verification passed.",
            "P3_PATIENT_INTAKE_FUNCTIONAL_CONTRACT_BASELINE.md",
            "P3_PATIENT_INTAKE_FUNCTIONAL_CONTRACT.md",
            "P3_SECURITY_PRODUCT_READINESS_GAP_AUDIT.md",
            "repository governance baseline"
        };

        AssertRequiredTokens(source, requiredTokens, "P3 patient intake functional contract verifier");
    }

    [Fact]
    public void RepositoryGovernanceBaseline_RunsPatientIntakeFunctionalContractVerifier()
    {
        var source = File.ReadAllText(GetScriptPath("validate-repo-governance-baseline.ps1"));

        Assert.Contains(
            "verify-p3-patient-intake-functional-contract.ps1",
            source,
            StringComparison.Ordinal);
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

    private static string GetProductDocPath(string fileName)
    {
        return Path.Combine(
            FindRepositoryRoot(),
            "docs",
            "product",
            fileName);
    }

    private static string GetScriptPath(string fileName)
    {
        return Path.Combine(
            FindRepositoryRoot(),
            "scripts",
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
