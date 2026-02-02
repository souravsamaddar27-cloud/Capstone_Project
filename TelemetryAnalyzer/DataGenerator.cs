using System;
using System.IO;
using System.Text;

namespace TelemetryAnalyzer
{
    /// <summary>
    /// Generates synthetic telemetry data for testing and demonstration
    /// Simulates realistic pacemaker device telemetry logs
    /// </summary>
    public class DataGenerator
    {
        private static readonly string[] DeviceIds =
        {
            "PM-2024-001", "PM-2024-002", "PM-2024-003", "PM-2024-004", "PM-2024-005",
            "PM-2024-006", "PM-2024-007", "PM-2024-008", "PM-2024-009", "PM-2024-010"
        };

        private static readonly string[] EventTypes = { "Info", "Warning", "Error", "Critical" };

        private static readonly string[] Severities = { "Low", "Medium", "High", "Critical" };

        private static readonly string[] ErrorMessages =
        {
            "Battery voltage low",
            "Lead impedance high",
            "Communication timeout",
            "Sensor malfunction",
            "Memory checksum error",
            "Pacing threshold exceeded",
            "Telemetry signal weak",
            "Device overheating",
            "Firmware update required",
            "Abnormal heart rhythm detected",
            "Power supply fluctuation",
            "Data corruption detected",
            "Connection lost",
            "Sensor calibration needed",
            "Temperature threshold exceeded"
        };

        /// <summary>
        /// Generates a CSV file with synthetic telemetry log entries
        /// </summary>
        /// <param name="outputPath">Path where CSV file will be created</param>
        /// <param name="recordCount">Number of telemetry records to generate (default: 50,000)</param>
        public static void GenerateSampleLogs(string outputPath, int recordCount = 50000)
        {
            Console.WriteLine($"🔧 Generating {recordCount:N0} sample telemetry records...");

            var random = new Random(42); // Fixed seed for reproducibility
            var sb = new StringBuilder();

            // CSV Header
            sb.AppendLine("DeviceId,Timestamp,EventType,Severity,Message,ErrorCode");

            var startDate = DateTime.Now.AddDays(-30); // Generate data for last 30 days

            for (int i = 0; i < recordCount; i++)
            {
                // Select random device
                var deviceId = DeviceIds[random.Next(DeviceIds.Length)];

                // Generate random timestamp within the last 30 days
                var timestamp = startDate.AddMinutes(random.Next(0, 43200)); // 30 days in minutes

                // Make some devices more error-prone for realistic scenarios
                // Devices ending in 003 and 007 have 40% error rate, others have 15%
                var errorProbability = deviceId.EndsWith("003") || deviceId.EndsWith("007") ? 0.4 : 0.15;
                var isError = random.NextDouble() < errorProbability;

                // Assign event type and severity based on error probability
                var eventType = isError ? EventTypes[random.Next(2, 4)] : EventTypes[random.Next(0, 2)];
                var severity = isError ? Severities[random.Next(2, 4)] : Severities[random.Next(0, 2)];
                var message = ErrorMessages[random.Next(ErrorMessages.Length)];
                var errorCode = random.Next(1000, 9999);

                // Append CSV row
                sb.AppendLine($"{deviceId},{timestamp:yyyy-MM-dd HH:mm:ss},{eventType},{severity},{message},{errorCode}");

                // Progress indicator every 10,000 records
                if (i % 10000 == 0 && i > 0)
                {
                    Console.WriteLine($"  ✓ Generated {i:N0} records...");
                }
            }

            // Ensure output directory exists
            var directory = Path.GetDirectoryName(outputPath);
            if (!string.IsNullOrEmpty(directory) && !Directory.Exists(directory))
            {
                Directory.CreateDirectory(directory);
            }

            // Write to file
            File.WriteAllText(outputPath, sb.ToString());

            Console.WriteLine($"✓ Sample telemetry data written to: {outputPath}");
            Console.WriteLine($"✓ File size: {new FileInfo(outputPath).Length / 1024:N0} KB\n");
        }
    }
}