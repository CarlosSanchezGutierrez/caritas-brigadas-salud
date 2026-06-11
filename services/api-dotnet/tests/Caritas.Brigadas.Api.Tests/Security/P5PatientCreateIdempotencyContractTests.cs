using Xunit;

namespace Caritas.Brigadas.Api.Tests.Security;

public sealed class P5PatientCreateIdempotencyContractTests
{
    [Fact]
    public void PatientWriteRepository_CreateAsync_EnforcesOfflineIdempotencyBeforeCreatingDuplicatePatient()
    {
        var source = File.ReadAllText(GetInfrastructurePath("Patients", "PatientWriteRepository.cs"));

        var requiredTokens = new[]
        {
            "FindExistingIdempotentPatientAsync",
            "existingIdempotentPatient",
            "return ToSummary(existingIdempotentPatient)",
            "NormalizeOptionalText(request.IdempotencyKey)",
            "patient.IdempotencyKey == idempotencyKey",
            "NormalizeOptionalText(request.ClientOperationId)",
            "patient.ClientOperationId == clientOperationId",
            "NormalizeOptionalText(request.LocalPatientId)",
            "patient.SourceBrigadeId == request.SourceBrigadeId.Value",
            "patient.LocalPatientId == localPatientId",
            "AsNoTracking",
            "FirstOrDefaultAsync(cancellationToken)"
        };

        AssertRequiredTokens(source, requiredTokens, "patient create idempotency");
    }

    [Fact]
    public void PatientWriteRepository_CreateAsync_PreservesOrganizationScopedIdempotency()
    {
        var source = File.ReadAllText(GetInfrastructurePath("Patients", "PatientWriteRepository.cs"));

        var requiredTokens = new[]
        {
            "Guid organizationId",
            "patient.OrganizationId == organizationId",
            "!patient.IsDeleted",
            "SourceBrigadeId"
        };

        AssertRequiredTokens(source, requiredTokens, "patient create idempotency organization scope");
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