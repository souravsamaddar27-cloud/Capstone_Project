using Metrics.Core.Enums;
using Metrics.Persistence.Entities;
namespace Metrics.Api.Contracts;

public sealed class CreateMetricDto
{
	public ActivityType ActivityType { get; set; }

	public string SubActivityName { get; set; } = default!;
	public long ExecutionTimeMs { get; set; }
	public long MemoryBytes { get; set; }
}
