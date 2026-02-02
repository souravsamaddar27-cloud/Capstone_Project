using Metrics.Core.Models;
using Metrics.Core.Enums;

namespace Metrics.Core.Models;

public sealed class CreateMetricModel
{
    public ActivityType ActivityType { get; init; }
    public string SubActivityName { get; init; } = string.Empty;
    public long ExecutionTimeMs { get; init; }
    public long MemoryBytes { get; init; }
}
