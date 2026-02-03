using System.Diagnostics;
using Metrics.Core.Enums;
using Metrics.Core.Interfaces;
using Metrics.Persistence.Db;
using Metrics.Persistence.Entities;

namespace Metrics.Persistence.Services;

public sealed class MetricsRecorder : IMetricsRecorder
{
    private readonly MetricsDbContext _db;

    public MetricsRecorder(MetricsDbContext db)
    {
        _db = db;
    }

    public async Task MeasureAsync(
        ActivityType activityType,
        string subActivityName,
        Func<Task> action,
        CancellationToken cancellationToken = default)
    {
        var stopwatch = Stopwatch.StartNew();
        var beforeMemory = GC.GetTotalMemory(false);

        await action();

        stopwatch.Stop();
        var afterMemory = GC.GetTotalMemory(false);

        var entity = new ExecutionMetricEntity
        {
            ActivityType = activityType,
            ActivityName = subActivityName,
            ExecutionTimeMs = stopwatch.Elapsed.TotalMilliseconds,
            MemoryBytes = afterMemory - beforeMemory,
            ExecutedAtUtc = DateTime.UtcNow
        };

        _db.ExecutionMetrics.Add(entity);
        await _db.SaveChangesAsync(cancellationToken);
    }
    public async Task<T> MeasureAsync<T>(
    ActivityType activityType,
    string subActivityName,
    Func<Task<T>> action,
    CancellationToken cancellationToken = default)
    {
        var beforeMemory = GC.GetTotalMemory(true);
        var stopwatch = System.Diagnostics.Stopwatch.StartNew();

        T result = await action();

        stopwatch.Stop();
        var afterMemory = GC.GetTotalMemory(true);

        var entity = new ExecutionMetricEntity
        {
            ActivityType = activityType,
            ActivityName = subActivityName,
            ExecutionTimeMs = stopwatch.ElapsedMilliseconds,
            MemoryBytes = afterMemory - beforeMemory,
            ExecutedAtUtc = DateTime.UtcNow
        };

        _db.ExecutionMetrics.Add(entity);
        await _db.SaveChangesAsync(cancellationToken);

        return result;
    }

}
