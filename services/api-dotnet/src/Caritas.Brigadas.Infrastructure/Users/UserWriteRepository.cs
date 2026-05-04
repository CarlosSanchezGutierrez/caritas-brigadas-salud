using Caritas.Brigadas.Application.Users;
using Caritas.Brigadas.Contracts.Users;
using Caritas.Brigadas.Domain.Entities;
using Caritas.Brigadas.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Caritas.Brigadas.Infrastructure.Users;

public sealed class UserWriteRepository : IUserWriteRepository
{
    private readonly CaritasDbContext _dbContext;

    public UserWriteRepository(CaritasDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<UserSummaryDto> CreateAsync(
        Guid organizationId,
        CreateUserRequest request,
        CancellationToken cancellationToken = default)
    {
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

        var normalizedEmail = string.IsNullOrWhiteSpace(request.Email)
            ? null
            : request.Email.Trim().ToLowerInvariant();

        var normalizedUsername = string.IsNullOrWhiteSpace(request.Username)
            ? null
            : request.Username.Trim();

        if (normalizedEmail is not null)
        {
            var emailExists = await _dbContext.Users
                .AsNoTracking()
                .AnyAsync(
                    user =>
                        user.OrganizationId == organizationId &&
                        user.Email == normalizedEmail &&
                        !user.IsDeleted,
                    cancellationToken);

            if (emailExists)
            {
                throw new InvalidOperationException("A user with the same email already exists in this organization.");
            }
        }

        if (normalizedUsername is not null)
        {
            var usernameExists = await _dbContext.Users
                .AsNoTracking()
                .AnyAsync(
                    user =>
                        user.OrganizationId == organizationId &&
                        user.Username == normalizedUsername &&
                        !user.IsDeleted,
                    cancellationToken);

            if (usernameExists)
            {
                throw new InvalidOperationException("A user with the same username already exists in this organization.");
            }
        }

        var user = new User(
            Guid.NewGuid(),
            organizationId,
            request.FullName,
            normalizedEmail,
            request.Phone,
            normalizedUsername);

        _dbContext.Users.Add(user);

        await _dbContext.SaveChangesAsync(cancellationToken);

        return new UserSummaryDto
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
        };
    }
}
