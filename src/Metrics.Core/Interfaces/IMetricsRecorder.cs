using Metrics.Core.Enums;

namespace Metrics.Core.Interfaces;

public interface IMetricsRecorder
{
    Task MeasureAsync(
        ActivityType activityType,
        string subActivityName,
        Func<Task> action,
        CancellationToken cancellationToken = default);
}
