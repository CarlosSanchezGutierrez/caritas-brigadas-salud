using System.Security.Claims;
using System.Text.Encodings.Web;
using Caritas.Brigadas.Contracts.Security;
using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Options;

namespace Caritas.Brigadas.Api.Security;

public sealed class DisabledAuthenticationHandler : AuthenticationHandler<AuthenticationSchemeOptions>
{
    private readonly IWebHostEnvironment _environment;

    public DisabledAuthenticationHandler(
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
            return Task.FromResult(AuthenticateResult.Fail(
                "Disabled authentication mode is only allowed in Development environment."));
        }

        var claims = new List<Claim>
        {
            new(CurrentUserClaimTypes.UserId, DisabledAuthenticationDefaults.UserId.ToString()),
            new(ClaimTypes.NameIdentifier, DisabledAuthenticationDefaults.UserId.ToString()),
            new(CurrentUserClaimTypes.OrganizationId, DisabledAuthenticationDefaults.OrganizationId.ToString()),
            new(ClaimTypes.Name, "Disabled Development User"),
            new(ClaimTypes.Email, "disabled-auth@localhost"),
            new(CurrentUserClaimTypes.Role, RoleCodes.SuperAdmin)
        };

        foreach (var permissionCode in PermissionCodes.All)
        {
            claims.Add(new Claim(CurrentUserClaimTypes.Permission, permissionCode));
        }

        var identity = new ClaimsIdentity(
            claims,
            DisabledAuthenticationDefaults.AuthenticationScheme);

        var principal = new ClaimsPrincipal(identity);

        var ticket = new AuthenticationTicket(
            principal,
            DisabledAuthenticationDefaults.AuthenticationScheme);

        return Task.FromResult(AuthenticateResult.Success(ticket));
    }
}