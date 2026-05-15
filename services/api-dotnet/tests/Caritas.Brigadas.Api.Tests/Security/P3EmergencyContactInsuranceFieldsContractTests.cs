using Xunit;

namespace Caritas.Brigadas.Api.Tests.Security;

public sealed class P3EmergencyContactInsuranceFieldsContractTests
{
    [Fact]
    public void EmergencyContactInsuranceFieldsContractBaseline_DefinesScope()
    {
        var source = File.ReadAllText(GetProductDocPath("P3_EMERGENCY_CONTACT_INSURANCE_FIELDS_CONTRACT_BASELINE.md"));

        var requiredTokens = new[]
        {
            "P3 Emergency Contact and Insurance Fields Contract Baseline",
            "hasEmergencyContact",
            "emergencyContactFullName",
            "emergencyContactPhoneNumber",
            "emergencyContactRelationship",
            "emergencyContactIsUnavailable",
            "emergencyContactUnavailableReason",
            "hasSocialSecurity",
            "socialSecurityProvider",
            "socialSecurityProviderOther",
            "hasPrivateInsurance",
            "privateInsuranceProvider",
            "insuranceInformationUnavailable",
            "insuranceInformationUnavailableReason",
            "IMSS",
            "ISSSTE",
            "OTHER",
            "emergency_contact_name_missing",
            "insurance_unavailable_reason_missing",
            "Acceptance criteria"
        };

        AssertRequiredTokens(source, requiredTokens, "P3 emergency contact and insurance baseline");
    }

    [Fact]
    public void EmergencyContactInsuranceFieldsContract_DefinesFrontendSyncAndLoggingBehavior()
    {
        var source = File.ReadAllText(GetProductDocPath("P3_EMERGENCY_CONTACT_INSURANCE_FIELDS_CONTRACT.md"));

        var requiredTokens = new[]
        {
            "P3 Emergency Contact and Insurance Fields Contract",
            "Frontend readiness impact: BLOCKS_FULL_FRONTEND",
            "Emergency contact fields",
            "Insurance and social security fields",
            "MVP decision",
            "Do not collect national social security numbers or policy numbers",
            "Emergency contact behavior",
            "Insurance/social security behavior",
            "Required enum values",
            "Validation rules",
            "Offline sync contract",
            "Spanish frontend labels",
            "Privacy and logging requirements",
            "Never log",
            "PARTIAL_PATIENT_DETAILS_FRONTEND_READY",
            "hasEmergencyContact",
            "emergencyContactFullName",
            "emergencyContactRelationship",
            "emergencyContactUnavailableReason",
            "hasSocialSecurity",
            "socialSecurityProvider",
            "socialSecurityProviderOther",
            "hasPrivateInsurance",
            "insuranceInformationUnavailableReason",
            "emergency_contact_name_missing",
            "social_security_provider_other_missing",
            "insurance_unavailable_reason_missing",
            "PayloadJson"
        };

        AssertRequiredTokens(source, requiredTokens, "P3 emergency contact and insurance contract");
    }

    [Fact]
    public void EmergencyContactInsuranceFieldsVerifier_RequiresPatientConsentGapAuditAndGovernanceReferences()
    {
        var source = File.ReadAllText(GetScriptPath("verify-p3-emergency-contact-insurance-fields-contract.ps1"));

        var requiredTokens = new[]
        {
            "P3 emergency contact and insurance fields contract verification passed.",
            "P3_EMERGENCY_CONTACT_INSURANCE_FIELDS_CONTRACT_BASELINE.md",
            "P3_EMERGENCY_CONTACT_INSURANCE_FIELDS_CONTRACT.md",
            "P3_PATIENT_INTAKE_FUNCTIONAL_CONTRACT.md",
            "P3_CONSENT_SIGNATURE_EVIDENCE_CONTRACT.md",
            "P3_SECURITY_PRODUCT_READINESS_GAP_AUDIT.md",
            "repository governance baseline"
        };

        AssertRequiredTokens(source, requiredTokens, "P3 emergency contact and insurance verifier");
    }

    [Fact]
    public void RepositoryGovernanceBaseline_RunsEmergencyContactInsuranceVerifier()
    {
        var source = File.ReadAllText(GetScriptPath("validate-repo-governance-baseline.ps1"));

        Assert.Contains(
            "verify-p3-emergency-contact-insurance-fields-contract.ps1",
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
