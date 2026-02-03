using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using TelemetryAnalyzer.Models;

namespace TelemetryAnalyzer.Services
{
    /// <summary>
    /// Reads and parses telemetry logs asynchronously
    /// Single Responsibility: Only handles file reading and parsing
    /// </summary>
    public class LogReader : ILogReader
    {
        public async Task<IEnumerable<TelemetryEvent>> ReadLogsAsync(string filePath)
        {
            if (!File.Exists(filePath))
            {
                throw new FileNotFoundException($"Log file not found: {filePath}");
            }

            var events = new List<TelemetryEvent>();

            // Read file asynchronously for better performance
            using (var reader = new StreamReader(filePath))
            {
                // Skip header line
                await reader.ReadLineAsync();

                string? line; // Added ? to indicate it can be null
                while ((line = await reader.ReadLineAsync()) != null)
                {
                    var telemetryEvent = ParseLogLine(line);
                    if (telemetryEvent != null)
                    {
                        events.Add(telemetryEvent);
                        ConfigurationProvider.Instance.IncrementProcessedCount();
                    }
                }
            }

            return events;
        }

        /// <summary>
        /// Parses a CSV line into a TelemetryEvent object
        /// </summary>
        private TelemetryEvent? ParseLogLine(string line) // Added ? to indicate return can be null
        {
            try
            {
                if (string.IsNullOrWhiteSpace(line))
                    return null;

                var parts = line.Split(',');

                if (parts.Length < 6)
                    return null;

                return new TelemetryEvent
                {
                    DeviceId = parts[0].Trim(),
                    Timestamp = DateTime.Parse(parts[1].Trim(), CultureInfo.InvariantCulture),
                    EventType = parts[2].Trim(),
                    Severity = parts[3].Trim(),
                    Message = parts[4].Trim(),
                    ErrorCode = int.TryParse(parts[5].Trim(), out int code) ? code : 0
                };
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error parsing line: {ex.Message}");
                return null;
            }
        }
    }
}