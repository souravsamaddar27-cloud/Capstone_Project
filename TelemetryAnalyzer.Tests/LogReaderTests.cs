using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using TelemetryAnalyzer.Services;

namespace TelemetryAnalyzer.Tests
{
    [TestClass]
    public class LogReaderTests
    {
        [TestMethod]
        public async Task ReadLogsAsync_ShouldParseCsvCorrectly()
        {
            // ARRANGE
            var reader = new LogReader();

            // Ensure test CSV is copied to output directory
            var filePath = Path.Combine(
                Directory.GetCurrentDirectory(),
                "test-logs.csv"
            );

            // ACT
            var events = await reader.ReadLogsAsync(filePath);

            // ASSERT
            Assert.AreEqual(1, events.Count());

            var telemetryEvent = events.First();
            Assert.AreEqual("PM-1", telemetryEvent.DeviceId);
            Assert.AreEqual("Error", telemetryEvent.EventType);
            Assert.AreEqual("High", telemetryEvent.Severity);
            Assert.AreEqual("Test Error", telemetryEvent.Message);
            Assert.AreEqual(1001, telemetryEvent.ErrorCode);
        }
    }
}
