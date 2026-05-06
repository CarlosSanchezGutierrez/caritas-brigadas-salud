using Caritas.Brigadas.Application.Security;
using Microsoft.AspNetCore.Authorization;

namespace Caritas.Brigadas.Api.Security;

public sealed class PermissionAuthorizationHandler : AuthorizationHandler<PermissionRequirement>
{
    private readonly ICurrentUserContext _currentUserContext;

    public PermissionAuthorizationHandler(ICurrentUserContext currentUserContext)
    {
        _currentUserContext = currentUserContext;
    }

    protected override Task HandleRequirementAsync(
        AuthorizationHandlerContext context,
        PermissionRequirement requirement)
    {
        if (!_currentUserContext.IsAuthenticated)
        {
            return Task.CompletedTask;
        }

        if (_currentUserContext.HasPermission(requirement.PermissionCode))
        {
            context.Succeed(requirement);
        }

        return Task.CompletedTask;
    }
}
