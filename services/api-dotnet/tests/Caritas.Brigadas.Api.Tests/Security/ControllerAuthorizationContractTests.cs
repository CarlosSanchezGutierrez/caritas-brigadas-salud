using System.Reflection;
using System.Text.RegularExpressions;
using Caritas.Brigadas.Application.Security;
using Xunit;

namespace Caritas.Brigadas.Api.Tests.Security;

public sealed class ControllerAuthorizationContractTests
{
    [Fact]
    public void ControllerHttpActions_HaveExplicitAuthorizationDecision()
    {
        var failures = new List<string>();

        foreach (var sourcePath in GetControllerSourcePaths())
        {
            var source = File.ReadAllText(sourcePath);
            var controllerHasAuthorizationDecision = ControllerHasAuthorizationDecision(source);

            foreach (Match httpAttribute in Regex.Matches(
                         source,
                         @"^\s*\[Http(Get|Post|Put|Patch|Delete)(\(|\])",
                         RegexOptions.Multiline))
            {
                var methodStart = source.IndexOf("public ", httpAttribute.Index, StringComparison.Ordinal);

                if (methodStart < 0)
                {
                    failures.Add($"{Path.GetFileName(sourcePath)}:{GetLineNumber(source, httpAttribute.Index)} has HTTP attribute without public action method.");
                    continue;
                }

                var actionAttributeBlock = source[httpAttribute.Index..methodStart];

                var actionHasAuthorizationDecision =
                    ContainsAuthorizationDecision(actionAttributeBlock) ||
                    controllerHasAuthorizationDecision;

                if (!actionHasAuthorizationDecision)
                {
                    failures.Add($"{Path.GetFileName(sourcePath)}:{GetLineNumber(source, httpAttribute.Index)} has HTTP action without [Authorize] or [AllowAnonymous].");
                }
            }
        }

        Assert.True(
            failures.Count == 0,
            "Every controller HTTP action must explicitly declare [Authorize] or [AllowAnonymous]." +
            Environment.NewLine +
            string.Join(Environment.NewLine, failures));
    }

    [Fact]
    public void ControllerAuthorizePolicies_ReferencePermissionCodesIncludedInAll()
    {
        var failures = new List<string>();

        foreach (var sourcePath in GetControllerSourcePaths())
        {
            var source = File.ReadAllText(sourcePath);

            foreach (Match match in Regex.Matches(
                         source,
                         @"Authorize\(Policy\s*=\s*PermissionCodes\.(?<member>[A-Za-z0-9_]+)\)"))
            {
                var memberName = match.Groups["member"].Value;
                var field = typeof(PermissionCodes).GetField(
                    memberName,
                    BindingFlags.Public | BindingFlags.Static);

                if (field is null)
                {
                    failures.Add($"{Path.GetFileName(sourcePath)}:{GetLineNumber(source, match.Index)} references missing PermissionCodes.{memberName}.");
                    continue;
                }

                var permissionCode = field.GetValue(null) as string;

                if (string.IsNullOrWhiteSpace(permissionCode))
                {
                    failures.Add($"{Path.GetFileName(sourcePath)}:{GetLineNumber(source, match.Index)} references non-string or empty PermissionCodes.{memberName}.");
                    continue;
                }

                if (!PermissionCodes.All.Contains(permissionCode, StringComparer.OrdinalIgnoreCase))
                {
                    failures.Add($"{Path.GetFileName(sourcePath)}:{GetLineNumber(source, match.Index)} references PermissionCodes.{memberName}, but '{permissionCode}' is not included in PermissionCodes.All.");
                }
            }
        }

        Assert.True(
            failures.Count == 0,
            "Every controller authorization policy must reference a PermissionCodes member included in PermissionCodes.All." +
            Environment.NewLine +
            string.Join(Environment.NewLine, failures));
    }

    [Fact]
    public void ControllerActionsUsingGlobalOnlyPermissions_HaveSuperAdminGuardrail()
    {
        var failures = new List<string>();

        foreach (var sourcePath in GetControllerSourcePaths())
        {
            var source = File.ReadAllText(sourcePath);

            foreach (Match match in Regex.Matches(
                         source,
                         @"Authorize\(Policy\s*=\s*PermissionCodes\.(?<member>[A-Za-z0-9_]+)\)"))
            {
                var memberName = match.Groups["member"].Value;
                var field = typeof(PermissionCodes).GetField(
                    memberName,
                    BindingFlags.Public | BindingFlags.Static);

                if (field?.GetValue(null) is not string permissionCode)
                {
                    continue;
                }

                if (!PermissionCodes.GlobalOnly.Contains(permissionCode, StringComparer.OrdinalIgnoreCase))
                {
                    continue;
                }

                var sourceContainsSuperAdminGuard =
                    source.Contains("IsSuperAdmin(User)", StringComparison.Ordinal) &&
                    source.Contains("return Forbid();", StringComparison.Ordinal);

                if (!sourceContainsSuperAdminGuard)
                {
                    failures.Add($"{Path.GetFileName(sourcePath)}:{GetLineNumber(source, match.Index)} uses global-only permission '{permissionCode}' without an explicit super-admin guardrail.");
                }
            }
        }

        Assert.True(
            failures.Count == 0,
            "Controller actions using global-only permissions must include an explicit super-admin guardrail." +
            Environment.NewLine +
            string.Join(Environment.NewLine, failures));
    }

    private static bool ControllerHasAuthorizationDecision(string source)
    {
        var classMatch = Regex.Match(
            source,
            @"public\s+sealed\s+class\s+\w+Controller\s*:",
            RegexOptions.Multiline);

        if (!classMatch.Success)
        {
            return false;
        }

        var prefixStart = source.LastIndexOf($"{Environment.NewLine}{Environment.NewLine}", classMatch.Index, StringComparison.Ordinal);
        prefixStart = prefixStart < 0 ? 0 : prefixStart;

        var classAttributeBlock = source[prefixStart..classMatch.Index];

        return ContainsAuthorizationDecision(classAttributeBlock);
    }

    private static bool ContainsAuthorizationDecision(string attributeBlock)
    {
        return Regex.IsMatch(
            attributeBlock,
            @"\[(Microsoft\.AspNetCore\.Authorization\.)?(Authorize|AllowAnonymous)(Attribute)?(\(|\])",
            RegexOptions.Multiline);
    }

    private static IReadOnlyCollection<string> GetControllerSourcePaths()
    {
        var repositoryRoot = FindRepositoryRoot();

        var controllersPath = Path.Combine(
            repositoryRoot,
            "services",
            "api-dotnet",
            "src",
            "Caritas.Brigadas.Api",
            "Controllers");

        return Directory
            .GetFiles(controllersPath, "*Controller.cs", SearchOption.AllDirectories)
            .Order(StringComparer.OrdinalIgnoreCase)
            .ToArray();
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
}