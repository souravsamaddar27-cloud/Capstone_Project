using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using System.Collections.Generic;
using Newtonsoft.Json.Linq;
using ApiHarvester.Models;

namespace ApiHarvester.Services
{
    public class OpenMeteoSource : IApiSource
    {
        public string Type => "openmeteo"; public string Name => "OpenMeteo";

        public async Task<ApiResult> FetchAsync(HttpClient http, SourceConfig cfg)
        {
            var json = await http.GetStringAsync(cfg.url);
            var root = JObject.Parse(json);

            var times = root["hourly"]?["time"]?.ToObject<List<string>>() ?? new List<string>();
            var temps = root["hourly"]?["temperature_2m"]?.ToObject<List<double>>() ?? new List<double>();

            var series = times.Zip(temps, (t, v) => new TimeValue { time = t, value = v }).ToList();
            var avg = temps.Count > 0 ? temps.Average() : 0.0;

            var payload = new OpenMeteoSummary
            {
                avgTemp = avg,
                hourlyTemp = series
            };

            return new ApiResult { SourceName = cfg.name ?? Name, SourceType = Type, Payload = payload };
        }
    }
}