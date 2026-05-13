using System.Text.RegularExpressions;
using Xunit;

namespace Caritas.Brigadas.Api.Tests.Security;

public sealed class P3EndpointAuthorizationContractTests
{
    private static readonly string[] AllowedPublicControllers =
    [
        "HealthController"
    ];

    [Fact]
    public void CurrentControllers_AreRepresentedByExactEndpointInventoryRows()
    {
        var inventory = File.ReadAllText(GetInventoryPath());
        var endpointSection = ExtractEndpointInventorySection(inventory);
        var controllerCells = GetEndpointInventoryControllerCells(endpointSection);
        var failures = new List<string>();

        foreach (var controllerPath in GetControllerSourcePaths())
        {
            var controllerName = Path.GetFileNameWithoutExtension(controllerPath);

            if (!controllerCells.Contains(controllerName, StringComparer.Ordinal))
            {
                failures.Add($"{controllerName} is missing an exact controller/area row in P3 tenant boundary authorization inventory.");
            }

            if (controllerCells.Contains($"Future {controllerName}", StringComparer.Ordinal))
            {
                failures.Add($"{controllerName} is a current controller but is still classified as Future in P3 tenant boundary authorization inventory.");
            }
        }

        Assert.True(
            failures.Count == 0,
            "Every current controller must have an exact controller/area row in P3 tenant boundary authorization inventory." +
            Environment.NewLine +
            string.Join(Environment.NewLine, failures));
    }

    [Fact]
    public void OnlyHealthController_CanBePublic()
    {
        var failures = new List<string>();

        foreach (var controllerPath in GetControllerSourcePaths())
        {
            var controllerName = Path.GetFileNameWithoutExtension(controllerPath);
            var source = File.ReadAllText(controllerPath);
            var hasAllowAnonymous = ContainsAllowAnonymous(source);

            if (AllowedPublicControllers.Contains(controllerName, StringComparer.Ordinal))
            {
                if (!hasAllowAnonymous)
                {
                    failures.Add($"{controllerName} is the only allowed public controller and must explicitly declare [AllowAnonymous].");
                }

                continue;
            }

            if (hasAllowAnonymous)
            {
                failures.Add($"{controllerName} is not allowed to declare [AllowAnonymous].");
            }
        }

        Assert.True(
            failures.Count == 0,
            "Only HealthController may be public. Protected controllers must not use [AllowAnonymous]." +
            Environment.NewLine +
            string.Join(Environment.NewLine, failures));
    }

    [Fact]
    public void ProtectedControllers_HaveAuthorizePolicyBasedOnPermissionCodes()
    {
        var failures = new List<string>();

        foreach (var controllerPath in GetControllerSourcePaths())
        {
            var controllerName = Path.GetFileNameWithoutExtension(controllerPath);

            if (AllowedPublicControllers.Contains(controllerName, StringComparer.Ordinal))
            {
                continue;
            }

            var source = File.ReadAllText(controllerPath);

            if (!ContainsAuthorize(source))
            {
                failures.Add($"{controllerName} does not declare [Authorize].");
                continue;
            }

            if (!ContainsPermissionCodePolicy(source))
            {
                failures.Add($"{controllerName} does not declare an [Authorize(Policy = PermissionCodes.*)] policy.");
            }
        }

        Assert.True(
            failures.Count == 0,
            "Every protected controller must declare authorization using PermissionCodes-based policies." +
            Environment.NewLine +
            string.Join(Environment.NewLine, failures));
    }

    [Fact]
    public void SystemInternalControllers_AreNotPublicAndAreInventoryClassified()
    {
        var inventory = File.ReadAllText(GetInventoryPath());
        var failures = new List<string>();

        var expectedSystemInternalControllers = new[]
        {
            "SecuritySeedController"
        };

        foreach (var controllerName in expectedSystemInternalControllers)
        {
            var controllerPath = GetControllerSourcePaths()
                .SingleOrDefault(path => Path.GetFileNameWithoutExtension(path).Equals(controllerName, StringComparison.Ordinal));

            if (controllerPath is null)
            {
                failures.Add($"{controllerName} source file was not found.");
                continue;
            }

            var source = File.ReadAllText(controllerPath);

            if (ContainsAllowAnonymous(source))
            {
                failures.Add($"{controllerName} must not be public or allow anonymous access.");
            }

            if (!ContainsAuthorize(source))
            {
                failures.Add($"{controllerName} must declare [Authorize].");
            }

            var inventoryPattern = @"\|\s*" +
                Regex.Escape(controllerName) +
                @"\s*\|[^\r\n]*\|\s*System/internal only\s*\|";

            if (!Regex.IsMatch(inventory, inventoryPattern))
            {
                failures.Add($"{controllerName} must be classified as System/internal only in the P3 inventory.");
            }
        }

        Assert.True(
            failures.Count == 0,
            "System/internal controllers must be non-public and classified correctly." +
            Environment.NewLine +
            string.Join(Environment.NewLine, failures));
    }

    [Fact]
    public void P3Inventory_EndpointRowsUseCanonicalClassificationsOnly()
    {
        var inventory = File.ReadAllText(GetInventoryPath());
        var endpointSection = ExtractEndpointInventorySection(inventory);

        var allowedClassifications = new HashSet<string>(StringComparer.Ordinal)
        {
            "Public",
            "Authenticated global",
            "Authenticated tenant-scoped",
            "Authenticated self-scoped",
            "Global-only",
            "System/internal only"
        };

        var failures = new List<string>();

        foreach (var row in GetMarkdownTableRows(endpointSection))
        {
            var columns = GetMarkdownTableColumns(row);

            if (columns.Length < 4)
            {
                failures.Add($"Endpoint inventory row must have 4 columns: {row}");
                continue;
            }

            var classification = columns[2];

            if (!allowedClassifications.Contains(classification))
            {
                failures.Add($"Endpoint inventory row uses non-canonical classification '{classification}': {row}");
            }
        }

        Assert.True(
            failures.Count == 0,
            "Endpoint inventory rows must use canonical P3 access classifications only." +
            Environment.NewLine +
            string.Join(Environment.NewLine, failures));
    }

    [Fact]
    public void P3Inventory_DoesNotContainKnownAmbiguousClassifications()
    {
        var inventory = File.ReadAllText(GetInventoryPath());

        var forbiddenTokens = new[]
        {
            "Public or system-safe",
            "Tenant-scoped with global guardrails",
            "Tenant-scoped/system constrained",
            "Authenticated tenant-scoped with global guardrails",
            "System constrained",
            "System-safe"
        };

        var failures = forbiddenTokens
            .Where(token => inventory.Contains(token, StringComparison.OrdinalIgnoreCase))
            .Select(token => $"Inventory contains forbidden ambiguous classification token: {token}")
            .ToList();

        Assert.True(
            failures.Count == 0,
            "P3 endpoint inventory must not contain ambiguous classification language." +
            Environment.NewLine +
            string.Join(Environment.NewLine, failures));
    }

    private static bool ContainsAllowAnonymous(string source)
    {
        return Regex.IsMatch(
            source,
            @"\[(Microsoft\.AspNetCore\.Authorization\.)?AllowAnonymous(Attribute)?(\(|\])",
            RegexOptions.Multiline);
    }

    private static bool ContainsAuthorize(string source)
    {
        return Regex.IsMatch(
            source,
            @"\[(Microsoft\.AspNetCore\.Authorization\.)?Authorize(Attribute)?(\(|\])",
            RegexOptions.Multiline);
    }

    private static bool ContainsPermissionCodePolicy(string source)
    {
        return Regex.IsMatch(
            source,
            @"Authorize\(Policy\s*=\s*PermissionCodes\.[A-Za-z0-9_]+\)",
            RegexOptions.Multiline);
    }

    private static string ExtractEndpointInventorySection(string inventory)
    {
        var match = Regex.Match(
            inventory,
            "(?s)## 6\\. Endpoint classification inventory.*?(?=## 7\\. Data domain tenant scope inventory)");

        if (!match.Success)
        {
            throw new InvalidOperationException("Endpoint classification inventory section was not found.");
        }

        return match.Value;
    }

    private static IReadOnlyCollection<string> GetEndpointInventoryControllerCells(string markdownSection)
    {
        return GetMarkdownTableRows(markdownSection)
            .Select(row => GetMarkdownTableColumns(row))
            .Where(columns => columns.Length >= 4)
            .Select(columns => columns[0])
            .ToHashSet(StringComparer.Ordinal);
    }

    private static IReadOnlyCollection<string> GetMarkdownTableRows(string markdownSection)
    {
        return markdownSection
            .Split(Environment.NewLine)
            .Where(line =>
                line.Trim().StartsWith("|", StringComparison.Ordinal) &&
                !line.Contains("|---", StringComparison.Ordinal) &&
                !line.Contains("Controller / area", StringComparison.Ordinal))
            .ToArray();
    }

    private static string[] GetMarkdownTableColumns(string row)
    {
        return row.Trim().Trim('|').Split('|').Select(column => column.Trim()).ToArray();
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