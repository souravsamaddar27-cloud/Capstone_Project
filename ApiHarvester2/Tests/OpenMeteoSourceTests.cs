using ApiHarvester.Models;
using ApiHarvester.Services;
using ApiHarvester.Tests;
using Newtonsoft.Json.Linq;
using System.Net.Http;
using System.Threading.Tasks;


namespace ApiHarvester.Tests
{
    public static class OpenMeteoSourceTests
    {
        public static async Task RunAsync()
        {
            var obj = new JObject
            {
                ["hourly"] = new JObject
                {
                    ["time"] = new JArray { "2026-01-29T10:00Z", "2026-01-29T11:00Z" },
                    ["temperature_2m"] = new JArray { 28.1, 29.9 }
                }
            };
            var json = obj.ToString();

            var http = new HttpClient(new FakeHandler(json));
            var src = new OpenMeteoSource();
            var cfg = new SourceConfig { name = "OpenMeteo", type = "openmeteo", url = "http://fake/openmeteo" };

            var result = await src.FetchAsync(http, cfg);

            Assert.IsNotNull(result, "ApiResult is null");
            Assert.AreEqual("openmeteo", result.SourceType.ToLower(), "Wrong SourceType");
            var payload = result.Payload as OpenMeteoSummary;
            Assert.IsNotNull(payload, "OpenMeteoSummary payload is null");
            Assert.AreEqual(2, payload.hourlyTemp.Count, "Hourly series length mismatch");
            Assert.IsTrue(payload.avgTemp > 28.0 && payload.avgTemp < 30.0, "Average temp out of range");
        }
    }
}
