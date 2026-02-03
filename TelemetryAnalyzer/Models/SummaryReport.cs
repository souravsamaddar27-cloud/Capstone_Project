using System.Collections.Generic;

namespace TelemetryAnalyzer.Models
{
    /// <summary>
    /// Contains aggregated statistics from telemetry analysis
    /// </summary>
    public class SummaryReport
    {
        public int TotalEvents { get; set; }
        public Dictionary<string, int> TopErrors { get; set; }
        public Dictionary<string, int> SeverityDistribution { get; set; }
        public Dictionary<string, DeviceStats> DeviceStatistics { get; set; }
        public DateTime GeneratedAt { get; set; }
        public double ProcessingTimeMs { get; set; }

        public SummaryReport()
        {
            TopErrors = new Dictionary<string, int>();
            SeverityDistribution = new Dictionary<string, int>();
            DeviceStatistics = new Dictionary<string, DeviceStats>();
            GeneratedAt = DateTime.Now;
        }
    }

    /// <summary>
    /// Statistics for an individual device
    /// </summary>
    public class DeviceStats
    {
        public string DeviceId { get; set; }
        public int EventCount { get; set; }
        public int ErrorCount { get; set; }
        public int WarningCount { get; set; }
        public DateTime LastSeen { get; set; }

        public DeviceStats()
        {
            DeviceId = string.Empty;
        }
    }
}