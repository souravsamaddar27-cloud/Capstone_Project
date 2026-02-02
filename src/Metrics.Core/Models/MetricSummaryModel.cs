using Metrics.Core.Enums;

namespace Metrics.Core.Models;

public sealed class MetricSummaryModel
{
    public ActivityType ActivityType { get; init; }
    public string SubActivityName { get; init; } = string.Empty;

    public double AverageExecutionMs { get; init; }
    public long MinExecutionMs { get; init; }
    public long MaxExecutionMs { get; init; }

    public double AverageMemoryBytes { get; init; }
    public long MinMemoryBytes { get; init; }
    public long MaxMemoryBytes { get; init; }
}
