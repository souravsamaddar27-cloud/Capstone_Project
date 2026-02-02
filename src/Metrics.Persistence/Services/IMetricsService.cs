using Metrics.Core.Models;

namespace Metrics.Core.Services;

public interface IMetricsService
{
    Task<IReadOnlyList<MetricSummaryModel>> GetSummaryAsync(
        CancellationToken cancellationToken = default);

    Task AddMetricAsync(
        CreateMetricModel model,
        CancellationToken cancellationToken = default);
}
