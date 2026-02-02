using System.Net.Http;
using System.Threading.Tasks;
using Newtonsoft.Json;
using System.Collections.Generic;
using ApiHarvester.Models;
using ApiHarvester.Services;
namespace ApiHarvester.Tests
{
    public static class CoinGeckoSourceTests
    {
        private static string json;

        public static async Task RunAsync()
        { // Arrange: sample CoinGecko markets JSON (subset of fields used) var coins = new List<object> { new { id = "bitcoin", symbol = "btc", name = "Bitcoin", market_cap = (double?)850000000000, price_change_percentage_24h = (double?) 1.2 }, new { id = "ethereum", symbol = "eth", name = "Ethereum", market_cap = (double?)320000000000, price_change_percentage_24h = (double?)-0.5 }, new { id = "tether", symbol = "usdt",name = "Tether", market_cap = (double?)96000000000, price_change_percentage_24h = (double?) 0.1 } }; var json = JsonConvert.SerializeObject(coins);

            var http = new HttpClient(new HttpFakeHandler(json));
            var src = new CoinGeckoSource();
            var cfg = new SourceConfig { name = "CoinGecko", type = "coingecko", url = "http://fake/cg" };

            // Act
            var result = await src.FetchAsync(http, cfg);

            // Assert
            Assert.IsNotNull(result, "ApiResult is null");
            Assert.AreEqual("coingecko", result.SourceType.ToLower(), "Wrong SourceType");
            var payload = result.Payload as CoinGeckoSummary;
            Assert.IsNotNull(payload, "CoinGeckoSummary payload is null");
            Assert.IsTrue(payload.topCoins.Count >= 3, "Not enough top coins");
            Assert.IsTrue(payload.change24hDistribution.gainers >= 1, "Gainers should be >= 1");
        }
    }
}