using Caritas.Brigadas.Api.Options;
using Xunit;

namespace Caritas.Brigadas.Api.Tests.Options;

public sealed class CaritasAuthenticationOptionsTests
{
    [Fact]
    public void ValidateForEnvironment_WhenDevelopmentModeInDevelopment_ReturnsNoErrors()
    {
        var options = new CaritasAuthenticationOptions
        {
            Mode = CaritasAuthenticationModes.Development
        };

        var errors = options.ValidateForEnvironment("Development");

        Assert.Empty(errors);
    }

    [Fact]
    public void ValidateForEnvironment_WhenDevelopmentModeOutsideDevelopment_ReturnsError()
    {
        var options = new CaritasAuthenticationOptions
        {
            Mode = CaritasAuthenticationModes.Development
        };

        var errors = options.ValidateForEnvironment("Production");

        Assert.Contains(errors, error =>
            error.Contains("Development authentication mode", StringComparison.OrdinalIgnoreCase));
    }

    [Fact]
    public void ValidateForEnvironment_WhenDisabledOutsideDevelopment_ReturnsError()
    {
        var options = new CaritasAuthenticationOptions
        {
            Mode = CaritasAuthenticationModes.Disabled
        };

        var errors = options.ValidateForEnvironment("Production");

        Assert.Contains(errors, error =>
            error.Contains("Disabled authentication mode", StringComparison.OrdinalIgnoreCase));
    }

    [Fact]
    public void ValidateForEnvironment_WhenJwtBearerWithoutAuthority_ReturnsError()
    {
        var options = new CaritasAuthenticationOptions
        {
            Mode = CaritasAuthenticationModes.JwtBearer,
            Audience = "caritas-brigadas-api"
        };

        var errors = options.ValidateForEnvironment("Production");

        Assert.Contains(errors, error =>
            error.Contains("Authority", StringComparison.OrdinalIgnoreCase));
    }

    [Fact]
    public void ValidateForEnvironment_WhenJwtBearerWithoutAudience_ReturnsError()
    {
        var options = new CaritasAuthenticationOptions
        {
            Mode = CaritasAuthenticationModes.JwtBearer,
            Authority = "https://issuer.example.com"
        };

        var errors = options.ValidateForEnvironment("Production");

        Assert.Contains(errors, error =>
            error.Contains("Audience", StringComparison.OrdinalIgnoreCase));
    }

    [Fact]
    public void ValidateForEnvironment_WhenJwtBearerHasAuthorityAndAudience_ReturnsNoErrors()
    {
        var options = new CaritasAuthenticationOptions
        {
            Mode = CaritasAuthenticationModes.JwtBearer,
            Authority = "https://issuer.example.com",
            Audience = "caritas-brigadas-api"
        };

        var errors = options.ValidateForEnvironment("Production");

        Assert.Empty(errors);
    }
}
