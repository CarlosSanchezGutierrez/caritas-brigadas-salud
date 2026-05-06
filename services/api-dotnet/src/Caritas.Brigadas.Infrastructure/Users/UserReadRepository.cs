using Caritas.Brigadas.Application.Users;
using Caritas.Brigadas.Contracts.Users;
using Caritas.Brigadas.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Caritas.Brigadas.Infrastructure.Users;

public sealed class UserReadRepository : IUserReadRepository
{
    private readonly CaritasDbContext _dbContext;

    public UserReadRepository(CaritasDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<IReadOnlyCollection<UserSummaryDto>> ListByOrganizationAsync(
        Guid organizationId,
        CancellationToken cancellationToken = default)
    {
        return await _dbContext.Users
            .AsNoTracking()
            .Where(user =>
                user.OrganizationId == organizationId &&
                !user.IsDeleted)
            .OrderBy(user => user.FullName)
            .Select(user => new UserSummaryDto
            {
                Id = user.Id,
                OrganizationId = user.OrganizationId,
                FullName = user.FullName,
                Email = user.Email,
                Phone = user.Phone,
                Username = user.Username,
                Status = user.Status,
                IsActive = user.IsActive,
                LastLoginAt = user.LastLoginAt
            })
            .ToListAsync(cancellationToken);
    }

    public async Task<UserSummaryDto?> GetByIdAsync(
        Guid userId,
        CancellationToken cancellationToken = default)
    {
        return await _dbContext.Users
            .AsNoTracking()
            .Where(user =>
                user.Id == userId &&
                !user.IsDeleted)
            .Select(user => new UserSummaryDto
            {
                Id = user.Id,
                OrganizationId = user.OrganizationId,
                FullName = user.FullName,
                Email = user.Email,
                Phone = user.Phone,
                Username = user.Username,
                Status = user.Status,
                IsActive = user.IsActive,
                LastLoginAt = user.LastLoginAt
            })
            .SingleOrDefaultAsync(cancellationToken);
    }
}
