using System.Collections.Generic;

namespace ApiHarvester2.Models
{
    public class CoinGeckoSummary
    {
        public List<NameValue> topCoins { get; set; } // Top coins by market cap
        public ChangeDistribution change24hDistribution { get; set; } // Gainers vs losers
    }
}
