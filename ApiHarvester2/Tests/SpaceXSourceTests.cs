using ApiHarvester.Models;
using ApiHarvester.Services;
using ApiHarvester.Tests;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;


namespace ApiHarvester.Tests
{
    public static class SpaceXSourceTests
    {
        public static async Task RunAsync()
        {
            // Arrange: sample SpaceX launches JSON
            var launches = new List<object>
            {
                new { date_utc = "2024-01-01T12:00:00Z", success = (bool?)true },
                new { date_utc = "2024-03-15T08:00:00Z", success = (bool?)false },
                new { date_utc = "2023-07-20T10:30:00Z", success = (bool?)true }
            };
            var json = JsonConvert.SerializeObject(launches); // <-- this line was missing

            var http = new HttpClient(new FakeHandler(json));
            var src = new SpaceXSource();
            var cfg = new SourceConfig { name = "SpaceX", type = "spacex", url = "http://fake/spacex" };

            // Act
            var result = await src.FetchAsync(http, cfg);

            // Assert
            Assert.IsNotNull(result, "ApiResult is null");
            Assert.AreEqual("spacex", result.SourceType.ToLower(), "Wrong SourceType");
            var payload = result.Payload as SpacexSummary;
            Assert.IsNotNull(payload, "SpacexSummary payload is null");
            Assert.IsTrue(payload.launchesPerYear.Count >= 1, "No launches per year computed");
            // successRate ~ 2/3
            Assert.IsTrue(payload.successRate > 0.6 && payload.successRate < 0.7, "SuccessRate out of expected range");
        }
    }
}
