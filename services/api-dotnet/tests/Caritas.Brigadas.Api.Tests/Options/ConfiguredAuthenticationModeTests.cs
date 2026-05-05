using Caritas.Brigadas.Api.Options;
using Xunit;

namespace Caritas.Brigadas.Api.Tests.Options;

public sealed class ConfiguredAuthenticationModeTests
{
    [Fact]
    public void CaritasAuthenticationModes_All_HasExpectedValues()
    {
        Assert.Contains(CaritasAuthenticationModes.Development, CaritasAuthenticationModes.All);
        Assert.Contains(CaritasAuthenticationModes.JwtBearer, CaritasAuthenticationModes.All);
        Assert.Contains(CaritasAuthenticationModes.Disabled, CaritasAuthenticationModes.All);
    }

    [Fact]
    public void CaritasAuthenticationModes_All_HasNoDuplicates()
    {
        var unique = CaritasAuthenticationModes.All
            .Distinct(StringComparer.OrdinalIgnoreCase)
            .Count();

        Assert.Equal(CaritasAuthenticationModes.All.Count, unique);
    }

    [Fact]
    public void ValidateForEnvironment_WhenUnsupportedMode_ReturnsError()
    {
        var options = new CaritasAuthenticationOptions
        {
            Mode = "UnknownMode"
        };

        var errors = options.ValidateForEnvironment("Development");

        Assert.Contains(errors, error =>
            error.Contains("not supported", StringComparison.OrdinalIgnoreCase));
    }
}
