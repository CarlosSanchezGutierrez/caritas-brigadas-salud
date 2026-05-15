using Xunit;

namespace Caritas.Brigadas.Api.Tests.Security;

public sealed class P3OpenApiFrontendContractFreezeTests
{
    [Fact]
    public void OpenApiFrontendContractFreezeBaseline_DefinesScope()
    {
        var source = File.ReadAllText(GetProductDocPath("P3_OPENAPI_FRONTEND_CONTRACT_FREEZE_BASELINE.md"));

        var requiredTokens = new[]
        {
            "P3 OpenAPI Frontend Contract Freeze Baseline",
            "/openapi/v1/openapi.json",
            "/swagger",
            "NEXT_PUBLIC_API_BASE_URL",
            "NEXT_PUBLIC_ENABLE_MOCK_API",
            "NEXT_PUBLIC_ENABLE_OFFLINE_MODE",
            "Authorization",
            "X-Correlation-Id",
            "Content-Type",
            "Accept",
            "Client applications must never connect directly to SQL Server.",
            "Acceptance criteria"
        };

        AssertRequiredTokens(source, requiredTokens, "P3 OpenAPI frontend contract freeze baseline");
    }

    [Fact]
    public void OpenApiFrontendContractFreeze_DefinesFrontendContract()
    {
        var source = File.ReadAllText(GetProductDocPath("P3_OPENAPI_FRONTEND_CONTRACT_FREEZE.md"));

        var requiredTokens = new[]
        {
            "P3 OpenAPI Frontend Contract Freeze",
            "FRONTEND_MVP_SCAFFOLD_READY",
            "UNBLOCKS_FRONTEND_MVP_SCAFFOLD",
            "/openapi/v1/openapi.json",
            "/swagger",
            "NEXT_PUBLIC_API_BASE_URL",
            "NEXT_PUBLIC_API_TIMEOUT_MS",
            "NEXT_PUBLIC_ENABLE_MOCK_API",
            "NEXT_PUBLIC_ENABLE_OFFLINE_MODE",
            "NEXT_PUBLIC_APP_ENVIRONMENT",
            "Authorization",
            "X-Correlation-Id",
            "Response envelope rules",
            "Error envelope rules",
            "Contract areas frozen for frontend MVP",
            "P3_PATIENT_INTAKE_FUNCTIONAL_CONTRACT.md",
            "P3_CONSENT_SIGNATURE_EVIDENCE_CONTRACT.md",
            "P3_EMERGENCY_CONTACT_INSURANCE_FIELDS_CONTRACT.md",
            "patient_identity_label_missing",
            "consent_signature_missing",
            "emergency_contact_relationship_missing",
            "social_security_provider_other_missing",
            "Never log",
            "PayloadJson",
            "Mock API mode is not allowed as",
            "Contract change control"
        };

        AssertRequiredTokens(source, requiredTokens, "P3 OpenAPI frontend contract freeze");
    }

    [Fact]
    public void SwaggerConfiguration_DefinesOpenApiDocumentAndUiRoutes()
    {
        var program = File.ReadAllText(GetApiSourcePath("Program.cs"));
        var swaggerExtensions = File.ReadAllText(GetApiSourcePath("Extensions", "SwaggerServiceExtensions.cs"));

        Assert.Contains("AddCaritasSwagger", program, StringComparison.Ordinal);
        Assert.Contains("UseCaritasSwagger", program, StringComparison.Ordinal);
        Assert.Contains("openapi/{documentName}/openapi.json", swaggerExtensions, StringComparison.Ordinal);
        Assert.Contains("RoutePrefix = \"swagger\"", swaggerExtensions, StringComparison.Ordinal);
        Assert.Contains("Brigadas de Salud API v1", swaggerExtensions, StringComparison.Ordinal);
    }

    [Fact]
    public void OpenApiFrontendContractFreezeVerifier_RequiresRelatedContractsAndGovernanceReferences()
    {
        var source = File.ReadAllText(GetScriptPath("verify-p3-openapi-frontend-contract-freeze.ps1"));

        var requiredTokens = new[]
        {
            "P3 OpenAPI frontend contract freeze verification passed.",
            "P3_OPENAPI_FRONTEND_CONTRACT_FREEZE_BASELINE.md",
            "P3_OPENAPI_FRONTEND_CONTRACT_FREEZE.md",
            "P3_PATIENT_INTAKE_FUNCTIONAL_CONTRACT.md",
            "P3_CONSENT_SIGNATURE_EVIDENCE_CONTRACT.md",
            "P3_EMERGENCY_CONTACT_INSURANCE_FIELDS_CONTRACT.md",
            "P3_SECURITY_PRODUCT_READINESS_GAP_AUDIT.md",
            "SwaggerServiceExtensions.cs",
            "repository governance baseline"
        };

        AssertRequiredTokens(source, requiredTokens, "P3 OpenAPI frontend contract freeze verifier");
    }

    [Fact]
    public void RepositoryGovernanceBaseline_RunsOpenApiFrontendContractFreezeVerifier()
    {
        var source = File.ReadAllText(GetScriptPath("validate-repo-governance-baseline.ps1"));

        Assert.Contains(
            "verify-p3-openapi-frontend-contract-freeze.ps1",
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

    private static string GetApiSourcePath(params string[] parts)
    {
        return Path.Combine(
            new[]
            {
                FindRepositoryRoot(),
                "services",
                "api-dotnet",
                "src",
                "Caritas.Brigadas.Api"
            }.Concat(parts).ToArray());
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
