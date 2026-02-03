using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using TelemetryAnalyzer.Models;
using TelemetryAnalyzer.Services;

namespace TelemetryAnalyzer.Tests
{
    [TestClass]
    public class AggregatorTests
    {
        [TestMethod]
        public void AggregateData_ShouldCountErrorsCorrectly()
        {
            // Arrange
            var events = new List<TelemetryEvent>
            {
                new TelemetryEvent
                {
                    DeviceId = "PM-001",
                    EventType = "Error",
                    Severity = "High",
                    Message = "Battery low",
                    Timestamp = DateTime.Now
                },
                new TelemetryEvent
                {
                    DeviceId = "PM-001",
                    EventType = "Error",
                    Severity = "High",
                    Message = "Battery low",
                    Timestamp = DateTime.Now
                }
            };

            var aggregator = new Aggregator();

            // Act
            var report = aggregator.AggregateData(events);

            // Assert
            Assert.AreEqual(2, report.TopErrors["Battery low"]);
        }
    }
}
