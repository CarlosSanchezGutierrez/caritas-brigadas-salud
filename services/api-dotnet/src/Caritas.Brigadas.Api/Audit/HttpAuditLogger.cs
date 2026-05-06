using Caritas.Brigadas.Application.Audit;
using Caritas.Brigadas.Application.Security;

namespace Caritas.Brigadas.Api.Audit;

public sealed class HttpAuditLogger : IAuditLogger
{
    private readonly IServiceProvider _serviceProvider;
    private readonly ICurrentUserContext _currentUserContext;
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly ILogger<HttpAuditLogger> _logger;

    public HttpAuditLogger(
        IServiceProvider serviceProvider,
        ICurrentUserContext currentUserContext,
        IHttpContextAccessor httpContextAccessor,
        ILogger<HttpAuditLogger> logger)
    {
        _serviceProvider = serviceProvider;
        _currentUserContext = currentUserContext;
        _httpContextAccessor = httpContextAccessor;
        _logger = logger;
    }

    public async Task LogAsync(
        Guid organizationId,
        string action,
        string entityName,
        Guid? entityId = null,
        string? detailsJson = null,
        CancellationToken cancellationToken = default)
    {
        if (organizationId == Guid.Empty ||
            string.IsNullOrWhiteSpace(action) ||
            string.IsNullOrWhiteSpace(entityName))
        {
            return;
        }

        var repository = _serviceProvider.GetService<IAuditLogWriteRepository>();

        if (repository is null)
        {
            return;
        }

        var httpContext = _httpContextAccessor.HttpContext;

        var command = new CreateAuditLogCommand
        {
            OrganizationId = organizationId,
            UserId = _currentUserContext.UserId,
            Action = action,
            EntityName = entityName,
            EntityId = entityId,
            DetailsJson = detailsJson,
            CorrelationId = httpContext?.TraceIdentifier,
            IpAddress = httpContext?.Connection.RemoteIpAddress?.ToString(),
            UserAgent = httpContext?.Request.Headers["User-Agent"].ToString(),
            OccurredAtUtc = DateTimeOffset.UtcNow
        };

        try
        {
            await repository.CreateAsync(command, cancellationToken);
        }
        catch (Exception exception)
        {
            _logger.LogWarning(
                exception,
                "Audit logging failed for action {Action} on entity {EntityName} with id {EntityId}.",
                action,
                entityName,
                entityId);
        }
    }
}
