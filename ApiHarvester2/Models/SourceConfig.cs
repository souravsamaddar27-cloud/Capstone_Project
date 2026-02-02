using System.Collections.Generic;

namespace ApiHarvester.Models
{
    public class SourceConfig
    {
        public string name { get; set; }  // e.g., "SpaceX"
        public string type { get; set; }  // e.g., "spacex", "coingecko", "openmeteo"
        public string url { get; set; }  // endpoint URL
    }

    public class SourcesRoot
    {
        public List<SourceConfig> sources { get; set; }
    }
}
