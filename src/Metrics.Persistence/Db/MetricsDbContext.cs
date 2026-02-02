//using Microsoft.EntityFrameworkCore;
//using Metrics.Persistence.Entities;

//namespace Metrics.Persistence.Db;

//public class MetricsDbContext : DbContext
//{
//    public MetricsDbContext(DbContextOptions<MetricsDbContext> options)
//        : base(options)
//    {
//    }

//    public DbSet<ExecutionMetricEntity> ExecutionMetrics
//          => Set<ExecutionMetricEntity>();


//    protected override void OnModelCreating(ModelBuilder modelBuilder)
//    {
//        base.OnModelCreating(modelBuilder);

//        modelBuilder.Entity<ExecutionMetricEntity>(entity =>
//        {
//            entity.ToTable("execution_metrics");

//            entity.HasIndex(e => e.ActivityType);
//            entity.HasIndex(e => e.ActivityName);
//            entity.HasIndex(e => e.ExecutedAtUtc);
//        });
//    }
//}

using Microsoft.EntityFrameworkCore;
using Metrics.Persistence.Entities;

namespace Metrics.Persistence.Db;

public sealed class MetricsDbContext : DbContext
{
    public MetricsDbContext(DbContextOptions<MetricsDbContext> options)
        : base(options)
    {
    }

    public DbSet<ExecutionMetricEntity> ExecutionMetrics
        => Set<ExecutionMetricEntity>();
}