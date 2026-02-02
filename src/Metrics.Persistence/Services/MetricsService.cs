using Microsoft.EntityFrameworkCore;
using Metrics.Core.Enums;
using Metrics.Core.Interfaces;
using Metrics.Core.Models;
using Metrics.Persistence.Db;
using Metrics.Persistence.Entities;

namespace Metrics.Persistence.Services;

public sealed class MetricsService : IMetricsService
{
    private readonly MetricsDbContext _db;

    public MetricsService(MetricsDbContext db)
    {
        _db = db;
    }

    public async Task<IReadOnlyList<MetricSummaryModel>> GetSummaryAsync(
        ActivityType? activityType,
        CancellationToken cancellationToken = default)
    {
        var query = _db.ExecutionMetrics.AsQueryable();

        if (activityType.HasValue)
            query = query.Where(x => x.ActivityType == activityType.Value);

        return await query
            .GroupBy(x => new { x.ActivityType, x.ActivityName })
            .Select(g => new MetricSummaryModel
            {
                ActivityType = g.Key.ActivityType,
                SubActivityName = g.Key.ActivityName,

                AverageExecutionMs = g.Average(x => (double)x.ExecutionTimeMs),
                MinExecutionMs = g.Min(x => x.ExecutionTimeMs),
                MaxExecutionMs = g.Max(x => x.ExecutionTimeMs),

                AverageMemoryBytes = g.Average(x => (double)x.MemoryBytes),
                MinMemoryBytes = g.Min(x => x.MemoryBytes),
                MaxMemoryBytes = g.Max(x => x.MemoryBytes)
            })
            .OrderBy(x => x.ActivityType)
            .ThenBy(x => x.SubActivityName)
            .ToListAsync(cancellationToken);
    }

    public async Task AddMetricAsync(
        CreateMetricModel model,
        CancellationToken cancellationToken = default)
    {
        var entity = new ExecutionMetricEntity
        {
            ActivityType = model.ActivityType,
            ActivityName = model.SubActivityName,
            ExecutionTimeMs = model.ExecutionTimeMs,
            MemoryBytes = model.MemoryBytes,
            ExecutedAtUtc = DateTime.UtcNow
        };

        _db.ExecutionMetrics.Add(entity);
        await _db.SaveChangesAsync(cancellationToken);
    }
}
