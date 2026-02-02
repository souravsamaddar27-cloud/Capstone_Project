using System;
using System.Collections.Generic;
using System.Linq;
using TelemetryAnalyzer.Models;

namespace TelemetryAnalyzer.Services
{
    /// <summary>
    /// Aggregates telemetry data using LINQ
    /// Single Responsibility: Only handles data aggregation
    /// </summary>
    public class Aggregator : IAggregator
    {
        public SummaryReport AggregateData(IEnumerable<TelemetryEvent> events)
        {
            var startTime = DateTime.Now;
            var eventsList = events.ToList(); // Materialize once for multiple operations

            var report = new SummaryReport
            {
                TotalEvents = eventsList.Count,
                GeneratedAt = DateTime.Now
            };

            // LINQ: Get top 10 error types by frequency
            report.TopErrors = eventsList
                .Where(e => e.EventType.Equals("Error", StringComparison.OrdinalIgnoreCase))
                .GroupBy(e => e.Message)
                .OrderByDescending(g => g.Count())
                .Take(10)

                .ToDictionary(g => g.Key, g => g.Count());

            // LINQ: Get severity distribution
            report.SeverityDistribution = eventsList
                .GroupBy(e => e.Severity)
                .ToDictionary(g => g.Key, g => g.Count());

            // LINQ: Get device statistics
            report.DeviceStatistics = eventsList
                .GroupBy(e => e.DeviceId)
                .ToDictionary(
                    g => g.Key,
                    g => new DeviceStats
                    {
                        DeviceId = g.Key,
                        EventCount = g.Count(),
                        ErrorCount = g.Count(e => e.EventType.Equals("Error", StringComparison.OrdinalIgnoreCase)),
                        WarningCount = g.Count(e => e.EventType.Equals("Warning", StringComparison.OrdinalIgnoreCase)),
                        LastSeen = g.Max(e => e.Timestamp)
                    }
                );

            // Calculate processing time
            report.ProcessingTimeMs = (DateTime.Now - startTime).TotalMilliseconds;

            return report;
        }
    }
}