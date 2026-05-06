using Caritas.Brigadas.Api.Options;
using Xunit;

namespace Caritas.Brigadas.Api.Tests.Options;

public sealed class JwtBearerAuthenticationSkeletonTests
{
    [Fact]
    public void JwtBearerMode_WhenAuthorityAndAudienceAreConfigured_IsValidForProduction()
    {
        var options = new CaritasAuthenticationOptions
        {
            Mode = CaritasAuthenticationModes.JwtBearer,
            Authority = "https://issuer.example.com",
            Audience = "caritas-brigadas-api",
            RequireHttpsMetadata = true
        };

        var errors = options.ValidateForEnvironment("Production");

        Assert.Empty(errors);
    }

    [Fact]
    public void JwtBearerMode_WhenValidAudiencesAreConfiguredWithoutAudience_IsValidForProduction()
    {
        var options = new CaritasAuthenticationOptions
        {
            Mode = CaritasAuthenticationModes.JwtBearer,
            Authority = "https://issuer.example.com",
            ValidAudiences = new[]
            {
                "caritas-brigadas-api",
                "caritas-mobile-app"
            },
            RequireHttpsMetadata = true
        };

        var errors = options.ValidateForEnvironment("Production");

        Assert.Empty(errors);
    }

    [Fact]
    public void DevelopmentMode_WhenRunningInProduction_IsInvalid()
    {
        var options = new CaritasAuthenticationOptions
        {
            Mode = CaritasAuthenticationModes.Development
        };

        var errors = options.ValidateForEnvironment("Production");

        Assert.NotEmpty(errors);
    }
}
