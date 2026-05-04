namespace Caritas.Brigadas.Contracts.Users;

public sealed record UserSummaryDto
{
    public Guid Id { get; init; }

    public Guid OrganizationId { get; init; }

    public string FullName { get; init; } = string.Empty;

    public string? Email { get; init; }

    public string? Phone { get; init; }

    public string? Username { get; init; }

    public string Status { get; init; } = string.Empty;

    public bool IsActive { get; init; }

    public DateTimeOffset? LastLoginAt { get; init; }
}
