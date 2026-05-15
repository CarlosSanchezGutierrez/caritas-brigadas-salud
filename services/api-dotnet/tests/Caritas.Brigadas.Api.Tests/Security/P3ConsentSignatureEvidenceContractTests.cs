using Xunit;

namespace Caritas.Brigadas.Api.Tests.Security;

public sealed class P3ConsentSignatureEvidenceContractTests
{
    [Fact]
    public void ConsentSignatureEvidenceContractBaseline_DefinesConsentSignatureScope()
    {
        var source = File.ReadAllText(GetProductDocPath("P3_CONSENT_SIGNATURE_EVIDENCE_CONTRACT_BASELINE.md"));

        var requiredTokens = new[]
        {
            "P3 Consent and Signature Evidence Contract Baseline",
            "privacy notice presentation",
            "patient or guardian signature",
            "consentDocumentId",
            "privacyNoticeVersion",
            "consentStatus",
            "signatureMethod",
            "signerType",
            "refusalReason",
            "unableToSignReason",
            "voidReason",
            "signatureSha256",
            "consentTextSnapshotHash",
            "ACCEPTED",
            "REFUSED",
            "UNABLE_TO_SIGN",
            "GUARDIAN_ACCEPTED",
            "DRAWN_SIGNATURE",
            "GUARDIAN_SIGNATURE",
            "signature evidence",
            "consent_document_created",
            "Consent and signature data must not be logged as raw request body.",
            "Acceptance criteria"
        };

        AssertRequiredTokens(source, requiredTokens, "P3 consent signature evidence contract baseline");
    }

    [Fact]
    public void ConsentSignatureEvidenceContract_DefinesFrontendSyncAndLoggingBehavior()
    {
        var source = File.ReadAllText(GetProductDocPath("P3_CONSENT_SIGNATURE_EVIDENCE_CONTRACT.md"));

        var requiredTokens = new[]
        {
            "P3 Consent and Signature Evidence Contract",
            "Frontend readiness impact: BLOCKS_FULL_FRONTEND",
            "Core consent workflow",
            "Required fields",
            "Consent status behavior",
            "Signature method behavior",
            "Storage contract",
            "Offline sync contract",
            "Validation rules",
            "Spanish frontend labels",
            "PARTIAL_CONSENT_FRONTEND_READY",
            "consent_document_created",
            "consent_signature_missing",
            "consent_guardian_relationship_missing",
            "consent_unable_to_sign_reason_missing",
            "refusalReason",
            "unableToSignReason",
            "voidReason",
            "REFUSED without refusalReason",
            "UNABLE_TO_SIGN without unableToSignReason",
            "VOIDED without voidReason",
            "drawnSignature",
            "guardian",
            "pendingSync",
            "Never log",
            "base64 signature",
            "PayloadJson"
        };

        AssertRequiredTokens(source, requiredTokens, "P3 consent signature evidence contract");
    }

    [Fact]
    public void ConsentSignatureEvidenceContractVerifier_RequiresPatientIntakeGapAuditAndGovernanceReferences()
    {
        var source = File.ReadAllText(GetScriptPath("verify-p3-consent-signature-evidence-contract.ps1"));

        var requiredTokens = new[]
        {
            "P3 consent and signature evidence contract verification passed.",
            "P3_CONSENT_SIGNATURE_EVIDENCE_CONTRACT_BASELINE.md",
            "P3_CONSENT_SIGNATURE_EVIDENCE_CONTRACT.md",
            "P3_PATIENT_INTAKE_FUNCTIONAL_CONTRACT.md",
            "P3_SECURITY_PRODUCT_READINESS_GAP_AUDIT.md",
            "repository governance baseline",
            "refusalReason",
            "unableToSignReason",
            "voidReason"
        };

        AssertRequiredTokens(source, requiredTokens, "P3 consent signature evidence contract verifier");
    }

    [Fact]
    public void RepositoryGovernanceBaseline_RunsConsentSignatureEvidenceContractVerifier()
    {
        var source = File.ReadAllText(GetScriptPath("validate-repo-governance-baseline.ps1"));

        Assert.Contains(
            "verify-p3-consent-signature-evidence-contract.ps1",
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
