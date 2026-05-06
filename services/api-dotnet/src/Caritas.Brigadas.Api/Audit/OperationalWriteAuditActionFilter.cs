using Caritas.Brigadas.Application.Audit;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace Caritas.Brigadas.Api.Audit;

public sealed class OperationalWriteAuditActionFilter : IAsyncActionFilter
{
    private readonly IAuditLogger _auditLogger;

    public OperationalWriteAuditActionFilter(IAuditLogger auditLogger)
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

        if (!OperationalWriteAuditActionMapper.TryMap(
                request.Method,
                request.Path.Value,
                out var action,
                out var entityName))
        {
            return;
        }

        if (!IsSuccessfulResult(executedContext.Result))
        {
            return;
        }

        var entityId = TryGetEntityId(executedContext.Result);
        var organizationId = TryGetOrganizationId(context, executedContext.Result, action, entityId);

        if (!organizationId.HasValue)
        {
            return;
        }

        await _auditLogger.LogAsync(
            organizationId.Value,
            action,
            entityName,
            entityId,
            detailsJson: null,
            context.HttpContext.RequestAborted);
    }

    private static Guid? TryGetOrganizationId(
        ActionExecutingContext context,
        IActionResult? result,
        string action,
        Guid? entityId)
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

        var organizationIdFromResponse = TryGetGuidPropertyFromData(result, "OrganizationId");

        if (organizationIdFromResponse.HasValue)
        {
            return organizationIdFromResponse;
        }

        if (string.Equals(action, AuditActionCodes.OrganizationCreate, StringComparison.OrdinalIgnoreCase))
        {
            return entityId;
        }

        return null;
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
        return TryGetGuidPropertyFromData(result, "Id");
    }

    private static Guid? TryGetGuidPropertyFromData(
        IActionResult? result,
        string propertyName)
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

        var targetProperty = data.GetType().GetProperty(propertyName);
        var targetValue = targetProperty?.GetValue(data);

        if (targetValue is Guid guid)
        {
            return guid;
        }

        return Guid.TryParse(targetValue?.ToString(), out var parsed)
            ? parsed
            : null;
    }
}
