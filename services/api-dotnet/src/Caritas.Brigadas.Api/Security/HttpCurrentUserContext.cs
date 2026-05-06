using System.Security.Claims;
using Caritas.Brigadas.Application.Security;

namespace Caritas.Brigadas.Api.Security;

public sealed class HttpCurrentUserContext : ICurrentUserContext
{
    private readonly IHttpContextAccessor _httpContextAccessor;

    public HttpCurrentUserContext(IHttpContextAccessor httpContextAccessor)
    {
        _httpContextAccessor = httpContextAccessor;
    }

    public bool IsAuthenticated =>
        User?.Identity?.IsAuthenticated == true;

    public Guid? UserId =>
        GetGuidClaim(CurrentUserClaimTypes.UserId) ??
        GetGuidClaim(ClaimTypes.NameIdentifier) ??
        GetGuidClaim(CurrentUserClaimTypes.LegacyUserId);

    public Guid? OrganizationId =>
        GetGuidClaim(CurrentUserClaimTypes.OrganizationId);

    public IReadOnlyCollection<string> Roles =>
        GetClaimValues(CurrentUserClaimTypes.RoleCode)
            .Concat(GetClaimValues(ClaimTypes.Role))
            .Concat(GetClaimValues(CurrentUserClaimTypes.LegacyRole))
            .Where(value => !string.IsNullOrWhiteSpace(value))
            .Select(value => value.Trim())
            .Distinct(StringComparer.OrdinalIgnoreCase)
            .ToArray();

    public IReadOnlyCollection<string> Permissions =>
        GetClaimValues(CurrentUserClaimTypes.PermissionCode)
            .Where(value => !string.IsNullOrWhiteSpace(value))
            .Select(value => value.Trim())
            .Distinct(StringComparer.OrdinalIgnoreCase)
            .ToArray();

    public bool IsInRole(string roleCode)
    {
        if (string.IsNullOrWhiteSpace(roleCode))
        {
            return false;
        }

        return Roles.Contains(roleCode.Trim(), StringComparer.OrdinalIgnoreCase);
    }

    public bool HasPermission(string permissionCode)
    {
        if (string.IsNullOrWhiteSpace(permissionCode))
        {
            return false;
        }

        if (IsInRole(RoleCodes.SuperAdmin))
        {
            return true;
        }

        return Permissions.Contains(permissionCode.Trim(), StringComparer.OrdinalIgnoreCase);
    }

    private ClaimsPrincipal? User =>
        _httpContextAccessor.HttpContext?.User;

    private Guid? GetGuidClaim(string claimType)
    {
        var value = User?.FindFirst(claimType)?.Value;

        return Guid.TryParse(value, out var guid)
            ? guid
            : null;
    }

    private IEnumerable<string> GetClaimValues(string claimType)
    {
        return User?.FindAll(claimType).Select(claim => claim.Value) ??
               Enumerable.Empty<string>();
    }
}
