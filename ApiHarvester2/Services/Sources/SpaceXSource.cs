using System;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using System.Collections.Generic;
using Newtonsoft.Json;
using ApiHarvester.Models;

namespace ApiHarvester.Services
{
    public class SpaceXSource : IApiSource
    {
        public string Type => "spacex"; 
        public string Name => "SpaceX";

        private class Launch { 
            public string date_utc { get; set; } 
            public bool? success { get; set; } 
        }

        public async Task<ApiResult> FetchAsync(HttpClient http, SourceConfig cfg)
        {
            var json = await http.GetStringAsync(cfg.url);
            var launches = JsonConvert.DeserializeObject<List<Launch>>(json) ?? new List<Launch>();

            var perYear = launches
                .Where(l => DateTime.TryParse(l.date_utc, out _))
                .GroupBy(l => DateTime.Parse(l.date_utc).Year)
                .Select(g => new YearCount { year = g.Key, count = g.Count() })
                .OrderBy(x => x.year)
                .ToList();

            var total = launches.Count;
            var successes = launches.Count(l => l.success == true);
            var successRate = total > 0 ? (double)successes / total : 0.0;

            var payload = new SpacexSummary
            {
                launchesPerYear = perYear,
                successRate = successRate
            };

            return new ApiResult { SourceName = cfg.name ?? Name, SourceType = Type, Payload = payload };
        }
    }
}