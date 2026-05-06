using Caritas.Brigadas.Api.Extensions;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Hosting;
using Xunit;

namespace Caritas.Brigadas.Api.Tests.Security;

public sealed class ProductionConfigurationValidationTests
{
    [Fact]
    public void ValidateProductionConfiguration_DoesNotThrow_InDevelopment()
    {
        var builder = WebApplication.CreateBuilder(new WebApplicationOptions
        {
            EnvironmentName = Environments.Development,
        });

        builder.Configuration["Authentication:Mode"] = "Development";

        var exception = Record.Exception(() => builder.ValidateProductionConfiguration());

        Assert.Null(exception);
    }

    [Fact]
    public void ValidateProductionConfiguration_Throws_WhenProductionUsesDevelopmentAuthentication()
    {
        var builder = CreateValidProductionBuilder();
        builder.Configuration["Authentication:Mode"] = "Development";

        var exception = Assert.Throws<InvalidOperationException>(
            () => builder.ValidateProductionConfiguration());

        Assert.Contains("Authentication:Mode", exception.Message, StringComparison.Ordinal);
    }

    [Fact]
    public void ValidateProductionConfiguration_Throws_WhenProductionHasNoSqlServerConnectionString()
    {
        var builder = CreateValidProductionBuilder();
        builder.Configuration["ConnectionStrings:SqlServer"] = string.Empty;

        var exception = Assert.Throws<InvalidOperationException>(
            () => builder.ValidateProductionConfiguration());

        Assert.Contains("SqlServer", exception.Message, StringComparison.Ordinal);
    }

    [Theory]
    [InlineData("Server=(localdb)\\\\MSSQLLocalDB;Database=CaritasBrigadas;Encrypt=True;TrustServerCertificate=False;")]
    [InlineData("Server=localhost;Database=CaritasBrigadas;Encrypt=True;TrustServerCertificate=False;")]
    [InlineData("Server=127.0.0.1;Database=CaritasBrigadas;Encrypt=True;TrustServerCertificate=False;")]
    [InlineData("Server=tcp:sql.example.org,1433;Database=CaritasBrigadas;Encrypt=False;TrustServerCertificate=False;")]
    [InlineData("Server=tcp:sql.example.org,1433;Database=CaritasBrigadas;Encrypt=True;TrustServerCertificate=True;")]
    [InlineData("Server=tcp:sql.example.org,1433;Database=CaritasBrigadas;TrustServerCertificate=False;")]
    public void ValidateProductionConfiguration_Throws_WhenProductionSqlServerConnectionStringIsUnsafe(string connectionString)
    {
        var builder = CreateValidProductionBuilder();
        builder.Configuration["ConnectionStrings:SqlServer"] = connectionString;

        var exception = Assert.Throws<InvalidOperationException>(
            () => builder.ValidateProductionConfiguration());

        Assert.Contains("SQL Server", exception.Message, StringComparison.OrdinalIgnoreCase);
    }

    [Fact]
    public void ValidateProductionConfiguration_Throws_WhenProductionCorsUsesLocalhost()
    {
        var builder = CreateValidProductionBuilder();
        builder.Configuration["Cors:AllowedOrigins:0"] = "https://localhost:3000";

        var exception = Assert.Throws<InvalidOperationException>(
            () => builder.ValidateProductionConfiguration());

        Assert.Contains("CORS", exception.Message, StringComparison.OrdinalIgnoreCase);
    }

    [Fact]
    public void ValidateProductionConfiguration_Throws_WhenProductionDisablesHttps()
    {
        var builder = CreateValidProductionBuilder();
        builder.Configuration["Security:RequireHttps"] = "false";

        var exception = Assert.Throws<InvalidOperationException>(
            () => builder.ValidateProductionConfiguration());

        Assert.Contains("RequireHttps", exception.Message, StringComparison.Ordinal);
    }

    [Fact]
    public void ValidateProductionConfiguration_Throws_WhenProductionAllowedHostsUsesWildcard()
    {
        var builder = CreateValidProductionBuilder();
        builder.Configuration["AllowedHosts"] = "*";

        var exception = Assert.Throws<InvalidOperationException>(
            () => builder.ValidateProductionConfiguration());

        Assert.Contains("AllowedHosts", exception.Message, StringComparison.Ordinal);
    }

    [Fact]
    public void ValidateProductionConfiguration_DoesNotThrow_WhenProductionConfigurationIsExplicit()
    {
        var builder = CreateValidProductionBuilder();

        var exception = Record.Exception(() => builder.ValidateProductionConfiguration());

        Assert.Null(exception);
    }

    private static WebApplicationBuilder CreateValidProductionBuilder()
    {
        var builder = WebApplication.CreateBuilder(new WebApplicationOptions
        {
            EnvironmentName = Environments.Production,
        });

        builder.Configuration["Authentication:Mode"] = "JwtBearer";
        builder.Configuration["ConnectionStrings:SqlServer"] = "Server=tcp:sql.example.org,1433;Database=CaritasBrigadas;User Id=caritas_app;Password=placeholder;Encrypt=True;TrustServerCertificate=False;";
        builder.Configuration["Cors:AllowedOrigins:0"] = "https://brigadas.caritas.example.org";
        builder.Configuration["Security:RequireHttps"] = "true";
        builder.Configuration["AllowedHosts"] = "brigadas.caritas.example.org";

        return builder;
    }
}
