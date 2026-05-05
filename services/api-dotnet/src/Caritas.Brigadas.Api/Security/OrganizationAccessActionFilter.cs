using Caritas.Brigadas.Application.Security;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace Caritas.Brigadas.Api.Security;

public sealed class OrganizationAccessActionFilter : IAsyncActionFilter
{
    private readonly IOrganizationAccessAuthorizer _organizationAccessAuthorizer;

    public OrganizationAccessActionFilter(
        IOrganizationAccessAuthorizer organizationAccessAuthorizer)
    {
        _organizationAccessAuthorizer = organizationAccessAuthorizer;
    }

    public async Task OnActionExecutionAsync(
        ActionExecutingContext context,
        ActionExecutionDelegate next)
    {
        var endpoint = context.HttpContext.GetEndpoint();

        if (endpoint?.Metadata.GetMetadata<IAllowAnonymous>() is not null)
        {
            await next();
            return;
        }

        var organizationId = TryGetOrganizationId(context);

        if (!organizationId.HasValue)
        {
            await next();
            return;
        }

        if (!_organizationAccessAuthorizer.CanAccessOrganization(organizationId.Value))
        {
            context.Result = new ForbidResult();
            return;
        }

        await next();
    }

    private static Guid? TryGetOrganizationId(ActionExecutingContext context)
    {
        if (context.RouteData.Values.TryGetValue("organizationId", out var routeValue) &&
            Guid.TryParse(routeValue?.ToString(), out var routeOrganizationId))
        {
            return routeOrganizationId;
        }

        if (context.ActionArguments.TryGetValue("organizationId", out var actionValue) &&
            actionValue is Guid actionOrganizationId)
        {
            return actionOrganizationId;
        }

        return null;
    }
}
