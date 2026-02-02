using Metrics.Core.Enums;
using Metrics.Core.Models;

namespace Metrics.Core.Interfaces;

public interface IMetricsService
{
    Task<IReadOnlyList<MetricSummaryModel>> GetSummaryAsync(
        ActivityType? activityType,
        CancellationToken cancellationToken = default);

    Task AddMetricAsync(
        CreateMetricModel model,
        CancellationToken cancellationToken = default);
}
