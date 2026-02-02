using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using System.Collections.Generic;
using Newtonsoft.Json;
using ApiHarvester.Models;

namespace ApiHarvester.Services
{
    public class CoinGeckoSource : IApiSource
    {
        public string Type => "coingecko";
        public string Name => "CoinGecko";

        private class MarketCoin
        {
            public string id { get; set; }
            public string symbol { get; set; }
            public string name { get; set; }
            public double? market_cap { get; set; }
            public double? price_change_percentage_24h { get; set; }
        }

        public async Task<ApiResult> FetchAsync(HttpClient http, SourceConfig cfg)
        {
            var req = new HttpRequestMessage(HttpMethod.Get, cfg.url);
            req.Headers.Accept.Clear();
            req.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            // Valid ProductInfo header; many services reject requests with no UA
            req.Headers.UserAgent.Clear();
            req.Headers.UserAgent.Add(new ProductInfoHeaderValue("ApiHarvester", "1.0"));

            var resp = await http.SendAsync(req);
            if (!resp.IsSuccessStatusCode)
            {
                // If CoinGecko responds 403, it's often due to rate limiting or missing UA.
                // We throw here and Program.RunMultiApiFlow will log and skip this source.
                throw new HttpRequestException($"HTTP {(int)resp.StatusCode} {resp.ReasonPhrase}");
            }

            var json = await resp.Content.ReadAsStringAsync();
            var coins = JsonConvert.DeserializeObject<List<MarketCoin>>(json) ?? new List<MarketCoin>();

            var top = coins
                .Where(c => c.market_cap.HasValue)
                .OrderByDescending(c => c.market_cap.Value)
                .Take(5)
                .Select(c => new NameValue { name = c.name, value = c.market_cap.Value })
                .ToList();

            var dist = new ChangeDistribution
            {
                gainers = coins.Count(c => (c.price_change_percentage_24h ?? 0) >= 0),
                losers = coins.Count(c => (c.price_change_percentage_24h ?? 0) < 0)
            };

            var payload = new CoinGeckoSummary
            {
                topCoins = top,
                change24hDistribution = dist
            };

            return new ApiResult { SourceName = cfg.name ?? Name, SourceType = Type, Payload = payload };
        }
    }
}
