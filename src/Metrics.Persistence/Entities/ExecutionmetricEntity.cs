using System.ComponentModel.DataAnnotations;
using Metrics.Core.Enums;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations.Schema;

using Metrics.Core.Models;

namespace Metrics.Persistence.Entities;

[Index(nameof(ActivityType), nameof(ActivityName))]
public sealed class ExecutionMetricEntity
{
    public long Id { get; set; }

    public ActivityType ActivityType { get; set; }


    public string ActivityName { get; set; } = default;

    public double ExecutionTimeMs { get; set; }


    public long MemoryBytes { get; set; }

    public DateTime ExecutedAtUtc { get; set; }
}