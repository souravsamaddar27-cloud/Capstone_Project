using ApiHarvester.Models;
using ApiHarvester.Services;
using ApiHarvester.Tests;
using System.Collections.Generic;


namespace ApiHarvester.Tests
{
    public static class MultiSourceProcessorTests
    {
        public static void Run()
        {
            // Arrange: build ApiResult list with minimal payloads
            var results = new List<ApiResult>
            {
                new ApiResult
                {
                    SourceName = "SpaceX",
                    SourceType = "spacex",
                    Payload = new SpacexSummary
                    {
                        launchesPerYear = new List<YearCount> { new YearCount { year = 2024, count = 2 } },
                        successRate = 0.5
                    }
                },
                new ApiResult
                {
                    SourceName = "OpenMeteo",
                    SourceType = "openmeteo",
                    Payload = new OpenMeteoSummary
                    {
                        avgTemp = 29.4,
                        hourlyTemp = new List<TimeValue> { new TimeValue { time = "2026-01-29T10:00Z", value = 28.0 } }
                    }
                }
            };

            // Act
            var summary = MultiSourceProcessor.Aggregate(results);

            // Assert
            Assert.IsNotNull(summary, "SummaryReport is null");
            Assert.IsNotNull(summary.SpaceX, "SpaceX summary missing");
            Assert.IsNotNull(summary.OpenMeteo, "OpenMeteo summary missing");
        }
    }
}
