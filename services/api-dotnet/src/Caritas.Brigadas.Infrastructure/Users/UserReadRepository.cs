using Caritas.Brigadas.Application.Users;
using Caritas.Brigadas.Contracts.Users;
using Caritas.Brigadas.Contracts.Api;
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

    public async Task<PaginatedResponse<UserSummaryDto>> ListByOrganizationAsync(
        Guid organizationId,
        PaginationRequest pagination,
        CancellationToken cancellationToken = default)
    {
        if (organizationId == Guid.Empty)
        {
            throw new ArgumentException("Organization id is required.", nameof(organizationId));
        }

        ArgumentNullException.ThrowIfNull(pagination);

        var pageNumber = pagination.NormalizedPageNumber;
        var pageSize = pagination.NormalizedPageSize;

        var query = _dbContext.Users
            .AsNoTracking()
            .Where(user =>
                user.OrganizationId == organizationId &&
                !user.IsDeleted);

        var totalCount = await query.CountAsync(cancellationToken);

        var items = await query
            .OrderBy(user => user.FullName)
            .ThenBy(user => user.Id)
            .Skip(pagination.Skip)
            .Take(pageSize)
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
            .ToArrayAsync(cancellationToken);

        return new PaginatedResponse<UserSummaryDto>
        {
            Items = items,
            PageNumber = pageNumber,
            PageSize = pageSize,
            TotalCount = totalCount
        };
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
