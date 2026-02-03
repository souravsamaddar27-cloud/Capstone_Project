using Metrics.Core.Enums;

namespace Metrics.Core.Interfaces;

public interface IMetricsRecorder
{
    Task<T> MeasureAsync<T>(
        ActivityType activityType,
        string subActivityName,
        Func<Task<T>> action,
        CancellationToken cancellationToken = default);

}
