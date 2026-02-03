using System.Collections.Generic;
using TelemetryAnalyzer.Models;

namespace TelemetryAnalyzer.Services
{
    /// <summary>
    /// Interface for aggregating telemetry data
    /// </summary>
    public interface IAggregator
    {
        SummaryReport AggregateData(IEnumerable<TelemetryEvent> events);
    }
}