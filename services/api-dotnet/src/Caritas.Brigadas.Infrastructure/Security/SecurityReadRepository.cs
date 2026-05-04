using Caritas.Brigadas.Application.Security;
using Caritas.Brigadas.Contracts.Security;
using Caritas.Brigadas.Domain.Entities;
using Caritas.Brigadas.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Caritas.Brigadas.Infrastructure.Security;

public sealed class SecurityReadRepository : ISecurityReadRepository
{
    private readonly CaritasDbContext _dbContext;

    public SecurityReadRepository(CaritasDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<IReadOnlyCollection<RoleSummaryDto>> ListRolesAsync(
        Guid organizationId,
        CancellationToken cancellationToken = default)
    {
        return await _dbContext.Roles
            .AsNoTracking()
            .Where(role =>
                role.OrganizationId == organizationId &&
                !role.IsDeleted)
            .OrderBy(role => role.Code)
            .Select(role => new RoleSummaryDto
            {
                Id = role.Id,
                OrganizationId = role.OrganizationId,
                Code = role.Code,
                Name = role.Name,
                Description = role.Description,
                IsSystemRole = role.IsSystemRole,
                Status = role.Status,
                IsActive = role.IsActive
            })
            .ToListAsync(cancellationToken);
    }

    public async Task<IReadOnlyCollection<UserRoleSummaryDto>> ListUserRolesAsync(
        Guid userId,
        CancellationToken cancellationToken = default)
    {
        var now = DateTimeOffset.UtcNow;

        return await _dbContext.UserRoles
            .AsNoTracking()
            .Where(userRole => userRole.UserId == userId)
            .Join(
                _dbContext.Roles.AsNoTracking(),
                userRole => userRole.RoleId,
                role => role.Id,
                (userRole, role) => new UserRoleSummaryDto
                {
                    Id = userRole.Id,
                    UserId = userRole.UserId,
                    RoleId = userRole.RoleId,
                    OrganizationId = userRole.OrganizationId,
                    RoleCode = role.Code,
                    RoleName = role.Name,
                    Status = userRole.Status,
                    AssignedAt = userRole.AssignedAt,
                    ExpiresAt = userRole.ExpiresAt,
                    IsActive = userRole.Status == UserRoleStatus.Active &&
                               (!userRole.ExpiresAt.HasValue || userRole.ExpiresAt.Value > now)
                })
            .OrderBy(userRole => userRole.RoleCode)
            .ToListAsync(cancellationToken);
    }
}
