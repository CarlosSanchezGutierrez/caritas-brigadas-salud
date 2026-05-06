namespace Caritas.Brigadas.Application.Security;

public interface IOrganizationAccessAuthorizer
{
    bool CanAccessOrganization(Guid organizationId);

    void EnsureCanAccessOrganization(Guid organizationId);
}
