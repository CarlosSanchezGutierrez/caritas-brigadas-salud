using Caritas.Brigadas.Application.Security;
using Caritas.Brigadas.Contracts.Security;
using Caritas.Brigadas.Domain.Entities;
using Caritas.Brigadas.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Caritas.Brigadas.Infrastructure.Security;

public sealed class UserRoleAssignmentRepository : IUserRoleAssignmentRepository
{
    private readonly CaritasDbContext _dbContext;

    public UserRoleAssignmentRepository(CaritasDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<UserRoleSummaryDto> AssignRoleAsync(
        Guid organizationId,
        Guid userId,
        AssignUserRoleRequest request,
        CancellationToken cancellationToken = default)
    {
        var now = DateTimeOffset.UtcNow;
        var roleCode = request.RoleCode.Trim().ToUpperInvariant();

        var organizationExists = await _dbContext.Organizations
            .AsNoTracking()
            .AnyAsync(
                organization =>
                    organization.Id == organizationId &&
                    !organization.IsDeleted,
                cancellationToken);

        if (!organizationExists)
        {
            throw new KeyNotFoundException("Organization was not found.");
        }

        var userExists = await _dbContext.Users
            .AsNoTracking()
            .AnyAsync(
                user =>
                    user.Id == userId &&
                    user.OrganizationId == organizationId &&
                    !user.IsDeleted,
                cancellationToken);

        if (!userExists)
        {
            throw new KeyNotFoundException("User was not found in this organization.");
        }

        var role = await _dbContext.Roles
            .AsNoTracking()
            .SingleOrDefaultAsync(
                item =>
                    item.OrganizationId == organizationId &&
                    item.Code == roleCode &&
                    !item.IsDeleted,
                cancellationToken);

        if (role is null)
        {
            throw new KeyNotFoundException("Role was not found in this organization.");
        }

        if (!role.IsActive)
        {
            throw new InvalidOperationException("Inactive roles cannot be assigned.");
        }

        var alreadyAssigned = await _dbContext.UserRoles
            .AsNoTracking()
            .AnyAsync(
                userRole =>
                    userRole.OrganizationId == organizationId &&
                    userRole.UserId == userId &&
                    userRole.RoleId == role.Id &&
                    userRole.Status == UserRoleStatus.Active &&
                    (!userRole.ExpiresAt.HasValue || userRole.ExpiresAt.Value > now),
                cancellationToken);

        if (alreadyAssigned)
        {
            throw new InvalidOperationException("User already has this active role.");
        }

        var assignment = new UserRole(
            Guid.NewGuid(),
            userId,
            role.Id,
            organizationId,
            now,
            expiresAt: request.ExpiresAt);

        _dbContext.UserRoles.Add(assignment);

        await _dbContext.SaveChangesAsync(cancellationToken);

        return new UserRoleSummaryDto
        {
            Id = assignment.Id,
            UserId = assignment.UserId,
            RoleId = assignment.RoleId,
            OrganizationId = assignment.OrganizationId,
            RoleCode = role.Code,
            RoleName = role.Name,
            Status = assignment.Status,
            AssignedAt = assignment.AssignedAt,
            ExpiresAt = assignment.ExpiresAt,
            IsActive = assignment.IsActiveAt(DateTimeOffset.UtcNow)
        };
    }
}
