using System.Security.Claims;
using System.Text.Encodings.Web;
using Caritas.Brigadas.Application.Security;
using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Options;

namespace Caritas.Brigadas.Api.Security;

public sealed class DevelopmentAuthenticationHandler : AuthenticationHandler<AuthenticationSchemeOptions>
{
    private readonly IWebHostEnvironment _environment;

    public DevelopmentAuthenticationHandler(
        IOptionsMonitor<AuthenticationSchemeOptions> options,
        ILoggerFactory logger,
        UrlEncoder encoder,
        IWebHostEnvironment environment)
        : base(options, logger, encoder)
    {
        _environment = environment;
    }

    protected override Task<AuthenticateResult> HandleAuthenticateAsync()
    {
        if (!_environment.IsDevelopment())
        {
            return Task.FromResult(AuthenticateResult.NoResult());
        }

        var userIdValue = Request.Headers[DevelopmentAuthenticationDefaults.UserIdHeaderName].FirstOrDefault();
        var organizationIdValue = Request.Headers[DevelopmentAuthenticationDefaults.OrganizationIdHeaderName].FirstOrDefault();

        if (string.IsNullOrWhiteSpace(userIdValue) &&
            string.IsNullOrWhiteSpace(organizationIdValue))
        {
            return Task.FromResult(AuthenticateResult.NoResult());
        }

        if (!Guid.TryParse(userIdValue, out var userId))
        {
            return Task.FromResult(AuthenticateResult.Fail("Development user id header is invalid."));
        }

        if (!Guid.TryParse(organizationIdValue, out var organizationId))
        {
            return Task.FromResult(AuthenticateResult.Fail("Development organization id header is invalid."));
        }

        var roles = SplitHeaderValues(
            Request.Headers[DevelopmentAuthenticationDefaults.RolesHeaderName].FirstOrDefault());

        if (roles.Count == 0)
        {
            roles.Add(RoleCodes.SuperAdmin);
        }

        var permissions = SplitHeaderValues(
            Request.Headers[DevelopmentAuthenticationDefaults.PermissionsHeaderName].FirstOrDefault());

        var name = Request.Headers[DevelopmentAuthenticationDefaults.NameHeaderName].FirstOrDefault();

        if (string.IsNullOrWhiteSpace(name))
        {
            name = "Development User";
        }

        var email = Request.Headers[DevelopmentAuthenticationDefaults.EmailHeaderName].FirstOrDefault();

        var claims = new List<Claim>
        {
            new(CurrentUserClaimTypes.UserId, userId.ToString()),
            new(ClaimTypes.NameIdentifier, userId.ToString()),
            new(CurrentUserClaimTypes.OrganizationId, organizationId.ToString()),
            new(ClaimTypes.Name, name)
        };

        if (!string.IsNullOrWhiteSpace(email))
        {
            claims.Add(new Claim(ClaimTypes.Email, email.Trim()));
        }

        foreach (var role in roles)
        {
            claims.Add(new Claim(CurrentUserClaimTypes.RoleCode, role));
            claims.Add(new Claim(ClaimTypes.Role, role));
        }

        foreach (var permission in permissions)
        {
            claims.Add(new Claim(CurrentUserClaimTypes.PermissionCode, permission));
        }

        var identity = new ClaimsIdentity(
            claims,
            DevelopmentAuthenticationDefaults.AuthenticationScheme);

        var principal = new ClaimsPrincipal(identity);

        var ticket = new AuthenticationTicket(
            principal,
            DevelopmentAuthenticationDefaults.AuthenticationScheme);

        return Task.FromResult(AuthenticateResult.Success(ticket));
    }

    private static List<string> SplitHeaderValues(string? value)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            return new List<string>();
        }

        return value
            .Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries)
            .Where(item => !string.IsNullOrWhiteSpace(item))
            .Distinct(StringComparer.OrdinalIgnoreCase)
            .ToList();
    }
}
