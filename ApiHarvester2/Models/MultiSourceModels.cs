using System.Collections.Generic;

namespace ApiHarvester.Models
{
    // Common helper DTOs
    public class YearCount { public int year { get; set; } public int count { get; set; } }
    public class NameCount { public string name { get; set; } public int count { get; set; } }
    public class NameValue { public string name { get; set; } public double value { get; set; } }
    public class ChangeDistribution { public int gainers { get; set; } public int losers { get; set; } }
    public class TimeValue { public string time { get; set; } public double value { get; set; } }

    // SpaceX summary (for v4 launches)
    public class SpacexSummary
    {
        public List<YearCount> launchesPerYear { get; set; }
        public double successRate { get; set; } // 0..1
    }

    // CoinGecko summary (market data)
    public class CoinGeckoSummary
    {
        public List<NameValue> topCoins { get; set; } // by market cap
        public ChangeDistribution change24hDistribution { get; set; }
    }

    // OpenMeteo summary (hourly temps and average)
    public class OpenMeteoSummary
    {
        public double avgTemp { get; set; }
        public List<TimeValue> hourlyTemp { get; set; }
    }

    // Unified result container used by adapters
    public class ApiResult
    {
        public string SourceName { get; set; }   // e.g., "SpaceX"
        public string SourceType { get; set; }   // spacex | coingecko | openmeteo
        public object Payload { get; set; }      // SpacexSummary | CoinGeckoSummary | OpenMeteoSummary
    }
}



//using System.Collections.Generic;

//namespace ApiHarvester.Models
//{
//    // Helper DTOs used by summaries
//    public class YearCount { public int year { get; set; } public int count { get; set; } }
//    public class NameValue { public string name { get; set; } public double value { get; set; } }
//    public class ChangeDistribution { public int gainers { get; set; } public int losers { get; set; } }
//    public class TimeValue { public string time { get; set; } public double value { get; set; } }

//    // Unified result container used by the source adapters
//    public class ApiResult
//    {
//        public string SourceName { get; set; }   // "SpaceX", "CoinGecko", "OpenMeteo"
//        public string SourceType { get; set; }   // "spacex", "coingecko", "openmeteo"
//        public object Payload { get; set; }   // SpacexSummary | CoinGeckoSummary | OpenMeteoSummary
//    }
//}




//using System.Collections.Generic;

//namespace ApiHarvester.Models
//{ // SpaceX public class YearCount { public int year { get; set; } public int count { get; set; } } public class NameCount { public string name { get; set; } public int count { get; set; } } public class SpacexSummary { public List<YearCount> launchesPerYear { get; set; } public double successRate { get; set; } // 0..1 }

//    // CoinGecko
//    public class NameValue { public string name { get; set; } public double value { get; set; } }
//    public class ChangeDistribution { public int gainers { get; set; } public int losers { get; set; } }
//    public class CoinGeckoSummary
//    {
//        public List<NameValue> topCoins { get; set; } // by market cap
//        public ChangeDistribution change24hDistribution { get; set; }
//    }

//    // OpenMeteo
//    public class TimeValue { public string time { get; set; } public double value { get; set; } }
//    public class OpenMeteoSummary
//    {
//        public double avgTemp { get; set; }
//        public List<TimeValue> hourlyTemp { get; set; }
//    }

//    // Unified result from any adapter
//    public class ApiResult
//    {
//        public string SourceName { get; set; }
//        public string SourceType { get; set; } // spacex, coingecko, openmeteo
//        public object Payload { get; set; }     // SpacexSummary | CoinGeckoSummary | OpenMeteoSummary
//    }
//}

