using System.Security.Claims;
using Caritas.Brigadas.Api.Audit;
using Caritas.Brigadas.Api.Security;
using Caritas.Brigadas.Application.Audit;
using Caritas.Brigadas.Application.Security;
using Caritas.Brigadas.Contracts.Audit;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging.Abstractions;
using Xunit;

namespace Caritas.Brigadas.Api.Tests.Audit;

public sealed class HttpAuditLoggerTests
{
    [Fact]
    public async Task LogAsync_WhenRepositoryExists_CreatesAuditLogCommand()
    {
        var organizationId = Guid.NewGuid();
        var userId = Guid.NewGuid();
        var entityId = Guid.NewGuid();

        var repository = new FakeAuditLogWriteRepository();

        var services = new ServiceCollection()
            .AddSingleton<IAuditLogWriteRepository>(repository)
            .BuildServiceProvider();

        var httpContext = new DefaultHttpContext
        {
            TraceIdentifier = "trace-test"
        };

        httpContext.Connection.RemoteIpAddress = System.Net.IPAddress.Parse("127.0.0.1");
        httpContext.Request.Headers.UserAgent = "unit-test-agent";

        var accessor = new HttpContextAccessor
        {
            HttpContext = httpContext
        };

        var currentUser = CreateCurrentUserContext(userId, organizationId);

        var logger = new HttpAuditLogger(
            services,
            currentUser,
            accessor,
            NullLogger<HttpAuditLogger>.Instance);

        await logger.LogAsync(
            organizationId,
            AuditActionCodes.PatientCreate,
            "Patient",
            entityId,
            "{\"source\":\"unit-test\"}", cancellationToken: TestContext.Current.CancellationToken);

        Assert.NotNull(repository.LastCommand);
        Assert.Equal(organizationId, repository.LastCommand!.OrganizationId);
        Assert.Equal(userId, repository.LastCommand.UserId);
        Assert.Equal(AuditActionCodes.PatientCreate, repository.LastCommand.Action);
        Assert.Equal("Patient", repository.LastCommand.EntityName);
        Assert.Equal(entityId, repository.LastCommand.EntityId);
        Assert.Equal("{\"source\":\"unit-test\"}", repository.LastCommand.DetailsJson);
        Assert.False(string.IsNullOrWhiteSpace(repository.LastCommand.CorrelationId));
    }

    [Fact]
    public async Task LogAsync_WhenRepositoryDoesNotExist_DoesNotThrow()
    {
        var services = new ServiceCollection().BuildServiceProvider();

        var accessor = new HttpContextAccessor
        {
            HttpContext = new DefaultHttpContext()
        };

        var currentUser = CreateCurrentUserContext(
            Guid.NewGuid(),
            Guid.NewGuid());

        var logger = new HttpAuditLogger(
            services,
            currentUser,
            accessor,
            NullLogger<HttpAuditLogger>.Instance);

        await logger.LogAsync(
            Guid.NewGuid(),
            AuditActionCodes.ReportSummaryRead,
            "Report", cancellationToken: TestContext.Current.CancellationToken);
    }

    [Fact]
    public void AuditActionCodes_All_HasNoDuplicates()
    {
        var unique = AuditActionCodes.All
            .Distinct(StringComparer.OrdinalIgnoreCase)
            .Count();

        Assert.Equal(AuditActionCodes.All.Count, unique);
    }

    private static HttpCurrentUserContext CreateCurrentUserContext(
        Guid userId,
        Guid organizationId)
    {
        var claims = new[]
        {
            new Claim(CurrentUserClaimTypes.UserId, userId.ToString()),
            new Claim(CurrentUserClaimTypes.OrganizationId, organizationId.ToString()),
            new Claim(CurrentUserClaimTypes.RoleCode, RoleCodes.SuperAdmin)
        };

        var principal = new ClaimsPrincipal(
            new ClaimsIdentity(claims, authenticationType: "Test"));

        var accessor = new HttpContextAccessor
        {
            HttpContext = new DefaultHttpContext
            {
                User = principal
            }
        };

        return new HttpCurrentUserContext(accessor);
    }

    private sealed class FakeAuditLogWriteRepository : IAuditLogWriteRepository
    {
        public CreateAuditLogCommand? LastCommand { get; private set; }

        public Task<AuditLogSummaryDto> CreateAsync(
            CreateAuditLogCommand command,
            CancellationToken cancellationToken = default)
        {
            LastCommand = command;

            var dto = new AuditLogSummaryDto
            {
                Id = Guid.NewGuid(),
                OrganizationId = command.OrganizationId,
                EntityName = command.EntityName,
                EntityId = command.EntityId,
                Action = command.Action,
                UserId = command.UserId,
                OccurredAtUtc = command.OccurredAtUtc ?? DateTimeOffset.UtcNow,
                CorrelationId = command.CorrelationId,
                IpAddress = command.IpAddress,
                DetailsJson = command.DetailsJson
            };

            return Task.FromResult(dto);
        }
    }
}



