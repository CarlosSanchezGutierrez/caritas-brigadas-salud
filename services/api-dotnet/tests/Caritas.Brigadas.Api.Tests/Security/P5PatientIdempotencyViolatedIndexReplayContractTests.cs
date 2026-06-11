using Xunit;

namespace Caritas.Brigadas.Api.Tests.Security;

public sealed class P5PatientIdempotencyViolatedIndexReplayContractTests
{
    [Fact]
    public void PatientWriteRepository_ReplaysConcurrentIdempotencyViolationByViolatedIndexIdentity()
    {
        var source = File.ReadAllText(GetInfrastructurePath("Patients", "PatientWriteRepository.cs"));

        var requiredTokens = new[]
        {
            "IdempotencyKeyUniqueIndexName",
            "ClientOperationIdUniqueIndexName",
            "LocalPatientUniqueIndexName",
            "FindExistingIdempotentPatientForUniqueViolationAsync",
            "GetPatientCreateIdempotencyUniqueIndexName",
            "FindExistingPatientByIdempotencyKeyAsync",
            "FindExistingPatientByClientOperationIdAsync",
            "FindExistingPatientByLocalPatientIdAsync",
            "violatedIndexName switch",
            "IdempotencyKeyUniqueIndexName => await FindExistingPatientByIdempotencyKeyAsync",
            "ClientOperationIdUniqueIndexName => await FindExistingPatientByClientOperationIdAsync",
            "LocalPatientUniqueIndexName => await FindExistingPatientByLocalPatientIdAsync",
            "return ToSummary(replayedPatient)"
        };

        AssertRequiredTokens(source, requiredTokens, "patient write repository violated-index replay");

        Assert.DoesNotContain(
            "var replayedPatient = await FindExistingIdempotentPatientAsync(",
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
            $"{label} contract is incomplete." +
            Environment.NewLine +
            string.Join(Environment.NewLine, failures));
    }

    private static string GetInfrastructurePath(params string[] segments)
    {
        return Path.Combine(new[]
        {
            FindRepositoryRoot(),
            "services",
            "api-dotnet",
            "src",
            "Caritas.Brigadas.Infrastructure"
        }.Concat(segments).ToArray());
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