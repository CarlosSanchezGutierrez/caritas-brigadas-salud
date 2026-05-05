namespace Caritas.Brigadas.Application.Audit;

public interface IAuditLogger
{
    Task LogAsync(
        Guid organizationId,
        string action,
        string entityName,
        Guid? entityId = null,
        string? detailsJson = null,
        CancellationToken cancellationToken = default);
}
