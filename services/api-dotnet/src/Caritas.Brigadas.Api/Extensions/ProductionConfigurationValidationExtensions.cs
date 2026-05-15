using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;

namespace Caritas.Brigadas.Api.Extensions;

public static class ProductionConfigurationValidationExtensions
{
    public static void ValidateProductionConfiguration(this WebApplicationBuilder builder)
    {
        ArgumentNullException.ThrowIfNull(builder);

        if (builder.Environment.IsDevelopment())
        {
            return;
        }

        var configuration = builder.Configuration;

        ValidateProductionAuthentication(configuration);
        ValidateProductionSqlServerConnectionString(configuration.GetConnectionString("SqlServer"));
        ValidateProductionCors(configuration);
        ValidateProductionHttps(configuration);
        ValidateProductionAllowedHosts(configuration);
        ValidateProductionRateLimiting(configuration);
    }

    private static void ValidateProductionAuthentication(IConfiguration configuration)
    {
        var authenticationMode = configuration["Authentication:Mode"];

        if (string.IsNullOrWhiteSpace(authenticationMode) ||
            string.Equals(authenticationMode, "Development", StringComparison.OrdinalIgnoreCase))
        {
            throw new InvalidOperationException(
                "Production requires Authentication:Mode to be configured to a non-Development provider.");
        }

        if (string.Equals(authenticationMode, "Disabled", StringComparison.OrdinalIgnoreCase))
        {
            throw new InvalidOperationException(
                "Disabled authentication mode is not allowed in Production environment.");
        }
    }

    private static void ValidateProductionSqlServerConnectionString(string? connectionString)
    {
        if (string.IsNullOrWhiteSpace(connectionString))
        {
            throw new InvalidOperationException(
                "Production requires ConnectionStrings:SqlServer to be configured from a secure secret source.");
        }

        if (connectionString.Contains("(localdb)", StringComparison.OrdinalIgnoreCase) ||
            connectionString.Contains("localhost", StringComparison.OrdinalIgnoreCase) ||
            connectionString.Contains("127.0.0.1", StringComparison.OrdinalIgnoreCase))
        {
            throw new InvalidOperationException(
                "Production SQL Server connection string cannot point to LocalDB, localhost, or loopback addresses.");
        }

        if (connectionString.Contains("TrustServerCertificate=True", StringComparison.OrdinalIgnoreCase))
        {
            throw new InvalidOperationException(
                "Production SQL Server connection string cannot use TrustServerCertificate=True.");
        }

        if (connectionString.Contains("Encrypt=False", StringComparison.OrdinalIgnoreCase))
        {
            throw new InvalidOperationException(
                "Production SQL Server connection string cannot use Encrypt=False.");
        }

        if (!connectionString.Contains("Encrypt=True", StringComparison.OrdinalIgnoreCase) &&
            !connectionString.Contains("Encrypt=Mandatory", StringComparison.OrdinalIgnoreCase) &&
            !connectionString.Contains("Encrypt=Strict", StringComparison.OrdinalIgnoreCase))
        {
            throw new InvalidOperationException(
                "Production SQL Server connection string must explicitly enable encryption.");
        }
    }

    private static void ValidateProductionCors(IConfiguration configuration)
    {
        var allowedOrigins = configuration
            .GetSection("Cors:AllowedOrigins")
            .Get<string[]>() ?? [];

        if (allowedOrigins.Length == 0)
        {
            throw new InvalidOperationException(
                "Production requires at least one explicit Cors:AllowedOrigins entry.");
        }

        if (allowedOrigins.Any(IsUnsafeCorsOrigin))
        {
            throw new InvalidOperationException(
                "Production CORS origins must be explicit HTTPS origins and cannot use localhost, loopback addresses, or wildcards.");
        }
    }

    private static void ValidateProductionHttps(IConfiguration configuration)
    {
        var requireHttps = configuration.GetValue("Security:RequireHttps", true);

        if (!requireHttps)
        {
            throw new InvalidOperationException(
                "Production requires Security:RequireHttps to be true.");
        }
    }

    private static void ValidateProductionAllowedHosts(IConfiguration configuration)
    {
        var allowedHosts = configuration["AllowedHosts"];

        if (string.IsNullOrWhiteSpace(allowedHosts) ||
            allowedHosts.Contains("*", StringComparison.Ordinal))
        {
            throw new InvalidOperationException(
                "Production requires AllowedHosts to be configured with explicit host names.");
        }
    }

    private static void ValidateProductionRateLimiting(IConfiguration configuration)
    {
        var enabled = configuration.GetValue("Security:RateLimiting:Enabled", true);

        if (!enabled)
        {
            throw new InvalidOperationException(
                "Production requires Security:RateLimiting:Enabled to be true.");
        }

        var permitLimit = configuration.GetValue("Security:RateLimiting:PermitLimit", 100);
        var windowMinutes = configuration.GetValue("Security:RateLimiting:WindowMinutes", 1);
        var queueLimit = configuration.GetValue("Security:RateLimiting:QueueLimit", 0);

        if (permitLimit <= 0)
        {
            throw new InvalidOperationException(
                "Production requires Security:RateLimiting:PermitLimit to be greater than zero.");
        }

        if (windowMinutes <= 0)
        {
            throw new InvalidOperationException(
                "Production requires Security:RateLimiting:WindowMinutes to be greater than zero.");
        }

        if (queueLimit < 0)
        {
            throw new InvalidOperationException(
                "Production requires Security:RateLimiting:QueueLimit to be zero or greater.");
        }
    }

    private static bool IsUnsafeCorsOrigin(string origin)
    {
        if (string.IsNullOrWhiteSpace(origin))
        {
            return true;
        }

        if (origin.Contains("*", StringComparison.Ordinal))
        {
            return true;
        }

        if (!Uri.TryCreate(origin, UriKind.Absolute, out var uri))
        {
            return true;
        }

        if (uri.Scheme != Uri.UriSchemeHttps)
        {
            return true;
        }

        return string.Equals(uri.Host, "localhost", StringComparison.OrdinalIgnoreCase) ||
            string.Equals(uri.Host, "127.0.0.1", StringComparison.OrdinalIgnoreCase) ||
            string.Equals(uri.Host, "::1", StringComparison.OrdinalIgnoreCase);
    }
}
