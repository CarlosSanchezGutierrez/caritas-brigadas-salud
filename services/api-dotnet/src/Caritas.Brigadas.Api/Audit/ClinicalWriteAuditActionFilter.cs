using Caritas.Brigadas.Application.Audit;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace Caritas.Brigadas.Api.Audit;

public sealed class ClinicalWriteAuditActionFilter : IAsyncActionFilter
{
    private readonly IAuditLogger _auditLogger;

    public ClinicalWriteAuditActionFilter(IAuditLogger auditLogger)
    {
        _auditLogger = auditLogger;
    }

    public async Task OnActionExecutionAsync(
        ActionExecutingContext context,
        ActionExecutionDelegate next)
    {
        var executedContext = await next();

        if (executedContext.Exception is not null)
        {
            return;
        }

        var request = context.HttpContext.Request;

        if (!ClinicalWriteAuditActionMapper.TryMap(
                request.Method,
                request.Path.Value,
                out var action,
                out var entityName))
        {
            return;
        }

        if (!TryGetOrganizationId(context, out var organizationId))
        {
            return;
        }

        if (!IsSuccessfulResult(executedContext.Result))
        {
            return;
        }

        var entityId = TryGetEntityId(executedContext.Result);

        await _auditLogger.LogAsync(
            organizationId,
            action,
            entityName,
            entityId,
            detailsJson: null,
            context.HttpContext.RequestAborted);
    }

    private static bool TryGetOrganizationId(
        ActionExecutingContext context,
        out Guid organizationId)
    {
        organizationId = Guid.Empty;

        if (context.RouteData.Values.TryGetValue("organizationId", out var routeValue) &&
            Guid.TryParse(routeValue?.ToString(), out var routeOrganizationId))
        {
            organizationId = routeOrganizationId;
            return true;
        }

        if (context.ActionArguments.TryGetValue("organizationId", out var actionValue) &&
            actionValue is Guid actionOrganizationId)
        {
            organizationId = actionOrganizationId;
            return true;
        }

        return false;
    }

    private static bool IsSuccessfulResult(IActionResult? result)
    {
        if (result is null)
        {
            return true;
        }

        if (result is ObjectResult objectResult)
        {
            var statusCode = objectResult.StatusCode ?? result switch
            {
                OkObjectResult => StatusCodes.Status200OK,
                CreatedResult => StatusCodes.Status201Created,
                CreatedAtActionResult => StatusCodes.Status201Created,
                CreatedAtRouteResult => StatusCodes.Status201Created,
                _ => StatusCodes.Status200OK
            };

            return statusCode is >= 200 and <= 299;
        }

        if (result is StatusCodeResult statusCodeResult)
        {
            return statusCodeResult.StatusCode is >= 200 and <= 299;
        }

        return false;
    }

    private static Guid? TryGetEntityId(IActionResult? result)
    {
        if (result is not ObjectResult objectResult ||
            objectResult.Value is null)
        {
            return null;
        }

        var value = objectResult.Value;
        var dataProperty = value.GetType().GetProperty("Data");

        var data = dataProperty?.GetValue(value);

        if (data is null)
        {
            return null;
        }

        var idProperty = data.GetType().GetProperty("Id");

        var idValue = idProperty?.GetValue(data);

        if (idValue is Guid id)
        {
            return id;
        }

        return Guid.TryParse(idValue?.ToString(), out var parsed)
            ? parsed
            : null;
    }
}
