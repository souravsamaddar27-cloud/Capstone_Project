using Metrics.Core.Enums;

namespace Metrics.Api.Contracts;

public sealed class MetricSummaryDto
{
    public ActivityType ActivityType { get; init; }

    public string SubActivityName { get; init; } = string.Empty;

    public double AverageExecutionMs { get; init; }   // double
    public double MinExecutionMs { get; init; }
    public double MaxExecutionMs { get; init; }


    public double AverageMemoryBytes { get; init; }   // double
    public long MinMemoryBytes { get; init; }         // long
    public long MaxMemoryBytes { get; init; }         // long
}

