using Caritas.Brigadas.Application.Security;

namespace Caritas.Brigadas.Api.Security;

public sealed class OrganizationAccessAuthorizer : IOrganizationAccessAuthorizer
{
    private readonly ICurrentUserContext _currentUserContext;

    public OrganizationAccessAuthorizer(ICurrentUserContext currentUserContext)
    {
        _currentUserContext = currentUserContext;
    }

    public bool CanAccessOrganization(Guid organizationId)
    {
        if (organizationId == Guid.Empty)
        {
            return false;
        }

        if (!_currentUserContext.IsAuthenticated)
        {
            return false;
        }

        if (_currentUserContext.IsInRole(RoleCodes.SuperAdmin))
        {
            return true;
        }

        return _currentUserContext.OrganizationId.HasValue &&
               _currentUserContext.OrganizationId.Value == organizationId;
    }

    public void EnsureCanAccessOrganization(Guid organizationId)
    {
        if (!CanAccessOrganization(organizationId))
        {
            throw new UnauthorizedAccessException("The current user cannot access this organization.");
        }
    }
}
