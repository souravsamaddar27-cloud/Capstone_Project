using System.Collections.Generic;

namespace ApiHarvester2.Models
{
    // Helper DTOs used by summaries
    public class YearCount { public int year { get; set; } public int count { get; set; } }
    public class NameValue { public string name { get; set; } public double value { get; set; } }
    public class ChangeDistribution { public int gainers { get; set; } public int losers { get; set; } }
    public class TimeValue { public string time { get; set; } public double value { get; set; } }

    // Unified result container used by the source adapters
    public class ApiResult
    {
        public string SourceName { get; set; }   // "SpaceX", "CoinGecko", "OpenMeteo"
        public string SourceType { get; set; }   // "spacex", "coingecko", "openmeteo"
        public object Payload { get; set; }   // SpacexSummary | CoinGeckoSummary | OpenMeteoSummary
    }
}
