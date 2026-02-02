using System.Net.Http;
using System.Threading.Tasks;
using ApiHarvester.Models;

namespace ApiHarvester.Services 
{ 
    public interface IApiSource 
    { 
        string Type { get; } // spacex | coingecko | openmeteo
        string Name { get; } // "SpaceX", "CoinGecko", "OpenMeteo"
        Task<ApiResult> FetchAsync(HttpClient http, SourceConfig cfg);
    } 
}