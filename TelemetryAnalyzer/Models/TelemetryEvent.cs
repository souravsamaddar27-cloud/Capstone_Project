namespace TelemetryAnalyzer.Models
{
    /// <summary>
    /// Represents a single telemetry event from a medical device
    /// </summary>
    public class TelemetryEvent
    {
        public string DeviceId { get; set; }
        public DateTime Timestamp { get; set; }
        public string EventType { get; set; }
        public string Severity { get; set; }
        public string Message { get; set; }
        public int ErrorCode { get; set; }

        // Constructor
        public TelemetryEvent()
        {
            DeviceId = string.Empty;
            EventType = string.Empty;
            Severity = string.Empty;
            Message = string.Empty;
        }
    }
}