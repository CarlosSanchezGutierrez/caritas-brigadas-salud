using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace Caritas.Brigadas.Infrastructure.Persistence;

public sealed class DesignTimeCaritasDbContextFactory : IDesignTimeDbContextFactory<CaritasDbContext>
{
    public CaritasDbContext CreateDbContext(string[] args)
    {
        var connectionString = Environment.GetEnvironmentVariable("CARITAS_SQLSERVER_CONNECTION")
            ?? "Server=localhost;Database=CaritasBrigadas_Local;Trusted_Connection=True;TrustServerCertificate=True;";

        var optionsBuilder = new DbContextOptionsBuilder<CaritasDbContext>();

        optionsBuilder.UseSqlServer(
            connectionString,
            sqlOptions =>
            {
                sqlOptions.MigrationsAssembly(typeof(CaritasDbContext).Assembly.FullName);
            });

        return new CaritasDbContext(optionsBuilder.Options);
    }
}
