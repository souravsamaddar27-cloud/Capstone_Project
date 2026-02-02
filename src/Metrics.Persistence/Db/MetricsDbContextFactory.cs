using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace Metrics.Persistence.Db;

public sealed class MetricsDbContextFactory
    : IDesignTimeDbContextFactory<MetricsDbContext>
{
    public MetricsDbContext CreateDbContext(string[] args)
    {
        var optionsBuilder = new DbContextOptionsBuilder<MetricsDbContext>();

        optionsBuilder.UseNpgsql(
            "Host=ep-lively-feather-a8pmfu4c.eastus2.azure.neon.tech;" +
            "Port=5432;" +
            "Database=neondb;" +
            "Username=neondb;" +
            "Password=npg_ZW6jz5KGQmYs;" +
            "SSL Mode=Require;" +
            "Trust Server Certificate=true"
        );


        return new MetricsDbContext(optionsBuilder.Options);
    }
}
