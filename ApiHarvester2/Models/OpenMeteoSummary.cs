//{ "sources": 
//        [ { "name": "SpaceX", "type": "spacex", "url": "https://api.spacexdata.com/v4/launches" }, 
//          { "name": "CoinGecko", "type": "coingecko", "url": "https://api.coingecko.com/api/v3/coins/markets?vs_currency=usd&order=market_cap_desc&per_page=50&page=1&sparkline=false&price_change_percentage=24h" }, { "name": "OpenMeteo", "type": "openmeteo", "url": "https://api.open-meteo.com/v1/forecast?latitude=28.6&longitude=77.2&hourly=temperature_2m&forecast_days=1&timezone=UTC" } ] 
//}


using System.Collections.Generic;

namespace ApiHarvester2.Models
{
    public class OpenMeteoSummary
    {
        public double avgTemp { get; set; }               // Average temperature (°C)
        public List<TimeValue> hourlyTemp { get; set; }   // Time series for hourly temps
    }
}
