using System.Text.RegularExpressions;
using Xunit;

namespace Caritas.Brigadas.Api.Tests.Security;

public sealed class P3TenantScopeContractTests
{
    [Fact]
    public void OrganizationScopedRoutes_DeclareOrganizationIdParameter()
    {
        var failures = new List<string>();

        foreach (var controllerPath in GetControllerSourcePaths())
        {
            var source = File.ReadAllText(controllerPath);

            foreach (var action in GetControllerActions(controllerPath, source))
            {
                if (!IsOrganizationScopedRoute(action.Source))
                {
                    continue;
                }

                if (!Regex.IsMatch(action.Source, @"\bGuid\s+organizationId\b"))
                {
                    failures.Add($"{Path.GetFileName(controllerPath)}:{GetLineNumber(source, action.HttpAttributeIndex)} has organization-scoped route without Guid organizationId parameter.");
                }
            }
        }

        Assert.True(
            failures.Count == 0,
            "Every organization-scoped endpoint must declare Guid organizationId." +
            Environment.NewLine +
            string.Join(Environment.NewLine, failures));
    }

    [Fact]
    public void OrganizationScopedRoutes_UseOrganizationIdBeyondRouteAndSignature()
    {
        var failures = new List<string>();

        foreach (var controllerPath in GetControllerSourcePaths())
        {
            var source = File.ReadAllText(controllerPath);

            foreach (var action in GetControllerActions(controllerPath, source))
            {
                if (!IsOrganizationScopedRoute(action.Source))
                {
                    continue;
                }

                var count = Regex.Matches(action.Source, @"\borganizationId\b").Count;

                if (count < 3)
                {
                    failures.Add($"{Path.GetFileName(controllerPath)}:{GetLineNumber(source, action.HttpAttributeIndex)} declares organizationId but does not appear to use it beyond route/signature.");
                }
            }
        }

        Assert.True(
            failures.Count == 0,
            "Organization-scoped endpoints must use organizationId in implementation, not only declare it in the route/signature." +
            Environment.NewLine +
            string.Join(Environment.NewLine, failures));
    }

    [Fact]
    public void OrganizationScopedReadByIdActions_ValidateOrganizationOwnership()
    {
        var failures = new List<string>();

        foreach (var controllerPath in GetControllerSourcePaths())
        {
            var source = File.ReadAllText(controllerPath);

            foreach (var action in GetControllerActions(controllerPath, source))
            {
                if (!IsOrganizationScopedRoute(action.Source))
                {
                    continue;
                }

                if (!IsReadByIdAction(action.Source))
                {
                    continue;
                }

                if (UsesOrganizationScopedGetById(action.Source))
                {
                    continue;
                }

                if (!ContainsOrganizationOwnershipCheck(action.Source))
                {
                    failures.Add($"{Path.GetFileName(controllerPath)}:{GetLineNumber(source, action.HttpAttributeIndex)} reads by id inside organization route without OrganizationId ownership check.");
                }
            }
        }

        Assert.True(
            failures.Count == 0,
            "Organization-scoped read-by-id actions must validate OrganizationId ownership or use an organization-scoped repository query." +
            Environment.NewLine +
            string.Join(Environment.NewLine, failures));
    }

    [Fact]
    public void OrganizationScopedListActions_UseOrganizationScopedQueries()
    {
        var failures = new List<string>();

        foreach (var controllerPath in GetControllerSourcePaths())
        {
            var source = File.ReadAllText(controllerPath);

            foreach (var action in GetControllerActions(controllerPath, source))
            {
                if (!IsOrganizationScopedRoute(action.Source))
                {
                    continue;
                }

                if (!IsHttpGetAction(action.Source))
                {
                    continue;
                }

                if (HasNonOrganizationRouteId(action.Source))
                {
                    continue;
                }

                if (!action.Source.Contains("ListByOrganizationAsync", StringComparison.Ordinal) &&
                    !action.Source.Contains("organizationId", StringComparison.Ordinal))
                {
                    failures.Add($"{Path.GetFileName(controllerPath)}:{GetLineNumber(source, action.HttpAttributeIndex)} list endpoint does not use organization-scoped query semantics.");
                }
            }
        }

        Assert.True(
            failures.Count == 0,
            "Organization-scoped list actions must use organization-scoped query semantics." +
            Environment.NewLine +
            string.Join(Environment.NewLine, failures));
    }

    [Fact]
    public void OrganizationScopedMutations_PassOrganizationIdToApplicationLayer()
    {
        var failures = new List<string>();

        foreach (var controllerPath in GetControllerSourcePaths())
        {
            var source = File.ReadAllText(controllerPath);

            foreach (var action in GetControllerActions(controllerPath, source))
            {
                if (!IsOrganizationScopedRoute(action.Source))
                {
                    continue;
                }

                if (!IsMutationAction(action.Source))
                {
                    continue;
                }

                var count = Regex.Matches(action.Source, @"\borganizationId\b").Count;

                if (count < 3)
                {
                    failures.Add($"{Path.GetFileName(controllerPath)}:{GetLineNumber(source, action.HttpAttributeIndex)} mutation endpoint does not appear to pass organizationId to the application layer.");
                }
            }
        }

        Assert.True(
            failures.Count == 0,
            "Organization-scoped mutations must pass organizationId into the application layer or enforce equivalent tenant ownership checks." +
            Environment.NewLine +
            string.Join(Environment.NewLine, failures));
    }

    [Fact]
    public void Controllers_DoNotUseKnownCrossTenantBypassPatterns()
    {
        var forbiddenPatterns = new[]
        {
            ".IgnoreQueryFilters(",
            "FromSqlRaw(",
            "ExecuteSqlRaw(",
            "SELECT *",
            "SELECT*"
        };

        var failures = new List<string>();

        foreach (var controllerPath in GetControllerSourcePaths())
        {
            var source = File.ReadAllText(controllerPath);

            foreach (var forbiddenPattern in forbiddenPatterns)
            {
                if (source.Contains(forbiddenPattern, StringComparison.OrdinalIgnoreCase))
                {
                    failures.Add($"{Path.GetFileName(controllerPath)} contains forbidden cross-tenant bypass pattern: {forbiddenPattern}");
                }
            }
        }

        Assert.True(
            failures.Count == 0,
            "Controllers must not use known cross-tenant bypass query patterns." +
            Environment.NewLine +
            string.Join(Environment.NewLine, failures));
    }

    [Fact]
    public void P3Inventory_DefinesTenantScopeRulesForCurrentDataDomains()
    {
        var inventory = File.ReadAllText(GetInventoryPath());

        var requiredTokens = new[]
        {
            "clinical.patients | Tenant-scoped by OrganizationId",
            "clinical.patient_visits | Tenant-scoped by OrganizationId",
            "clinical.service_encounters | Tenant-scoped by OrganizationId",
            "forms.form_responses | Tenant-scoped by OrganizationId",
            "documents.document_signatures | Tenant-scoped by OrganizationId",
            "sync.sync_batches | Tenant-scoped by OrganizationId",
            "sync.sync_events | Tenant-scoped by OrganizationId"
        };

        var failures = requiredTokens
            .Where(token => !inventory.Contains(token, StringComparison.Ordinal))
            .Select(token => $"Inventory is missing tenant scope rule: {token}")
            .ToList();

        Assert.True(
            failures.Count == 0,
            "P3 tenant boundary inventory must define tenant scope rules for current data domains." +
            Environment.NewLine +
            string.Join(Environment.NewLine, failures));
    }

    private static bool IsOrganizationScopedRoute(string actionSource)
    {
        return actionSource.Contains("organizations/{organizationId:guid}", StringComparison.OrdinalIgnoreCase);
    }

    private static bool IsHttpGetAction(string actionSource)
    {
        return Regex.IsMatch(actionSource, @"^\s*\[HttpGet", RegexOptions.Multiline);
    }

    private static bool IsMutationAction(string actionSource)
    {
        return Regex.IsMatch(actionSource, @"^\s*\[Http(Post|Put|Patch|Delete)", RegexOptions.Multiline);
    }

    private static bool IsReadByIdAction(string actionSource)
    {
        return IsHttpGetAction(actionSource) &&
            HasNonOrganizationRouteId(actionSource) &&
            actionSource.Contains("GetByIdAsync", StringComparison.Ordinal);
    }

    private static bool HasNonOrganizationRouteId(string actionSource)
    {
        var routeIdMatches = Regex
            .Matches(actionSource, @"\{(?<name>[A-Za-z0-9_]+Id):guid\}")
            .Cast<Match>()
            .Select(match => match.Groups["name"].Value)
            .Where(name => !name.Equals("organizationId", StringComparison.Ordinal))
            .ToArray();

        return routeIdMatches.Length > 0;
    }

    private static bool UsesOrganizationScopedGetById(string actionSource)
    {
        return Regex.IsMatch(
            actionSource,
            @"GetByIdAsync\s*\(\s*organizationId\s*,",
            RegexOptions.Multiline);
    }

    private static bool ContainsOrganizationOwnershipCheck(string actionSource)
    {
        return Regex.IsMatch(
                actionSource,
                @"\.OrganizationId\s*!=\s*organizationId",
                RegexOptions.Multiline) ||
            Regex.IsMatch(
                actionSource,
                @"\.OrganizationId\s*==\s*organizationId",
                RegexOptions.Multiline);
    }

    private static IReadOnlyCollection<ControllerActionSource> GetControllerActions(
        string sourcePath,
        string source)
    {
        var httpAttributes = Regex.Matches(
                source,
                @"^\s*\[Http(Get|Post|Put|Patch|Delete)(\(|\])",
                RegexOptions.Multiline)
            .Cast<Match>()
            .ToArray();

        var actions = new List<ControllerActionSource>();

        for (var index = 0; index < httpAttributes.Length; index++)
        {
            var httpAttribute = httpAttributes[index];
            var nextHttpAttributeIndex = index + 1 < httpAttributes.Length
                ? httpAttributes[index + 1].Index
                : source.Length;

            var methodStart = source.IndexOf(
                "public ",
                httpAttribute.Index,
                StringComparison.Ordinal);

            if (methodStart < 0 || methodStart >= nextHttpAttributeIndex)
            {
                throw new InvalidOperationException(
                    $"{Path.GetFileName(sourcePath)}:{GetLineNumber(source, httpAttribute.Index)} has an HTTP attribute without a public action method.");
            }

            var actionSource = source[httpAttribute.Index..nextHttpAttributeIndex];

            actions.Add(new ControllerActionSource(
                httpAttribute.Index,
                actionSource));
        }

        return actions;
    }

    private static IReadOnlyCollection<string> GetControllerSourcePaths()
    {
        var controllersPath = Path.Combine(
            FindRepositoryRoot(),
            "services",
            "api-dotnet",
            "src",
            "Caritas.Brigadas.Api",
            "Controllers");

        return Directory
            .GetFiles(controllersPath, "*Controller.cs", SearchOption.TopDirectoryOnly)
            .Order(StringComparer.OrdinalIgnoreCase)
            .ToArray();
    }

    private static string GetInventoryPath()
    {
        return Path.Combine(
            FindRepositoryRoot(),
            "docs",
            "backend",
            "P3_TENANT_BOUNDARY_AUTHORIZATION_INVENTORY.md");
    }

    private static int GetLineNumber(string source, int index)
    {
        return source[..index].Count(character => character == '\n') + 1;
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

    private sealed record ControllerActionSource(
        int HttpAttributeIndex,
        string Source);
}