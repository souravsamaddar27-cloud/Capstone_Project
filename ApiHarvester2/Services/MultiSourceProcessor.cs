using System.Collections.Generic;
using ApiHarvester.Models;

namespace ApiHarvester.Services { 
    public static class MultiSourceProcessor { 
        // Merge payloads from adapters into SummaryReport
        public static SummaryReport Aggregate(List<ApiResult> results) { 
            var summary = new SummaryReport(); 
            foreach (var r in results) { 
                switch (r.SourceType) { 
                    case "spacex": summary.SpaceX = r.Payload as SpacexSummary; 
                        break; 
                    case "coingecko": summary.CoinGecko = r.Payload as CoinGeckoSummary; 
                        break; 
                    case "openmeteo": summary.OpenMeteo = r.Payload as OpenMeteoSummary; 
                        break; 
                } 
            } 
            return summary;
        } 
    } 
}