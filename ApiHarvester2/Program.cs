using System;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using System.Collections.Generic;
using Newtonsoft.Json;
using ApiHarvester.Models;
using ApiHarvester.Services;
using ApiHarvester.Tests; // remove if you don't have the in-project test harness
using System.Net.Http.Headers;

class Program
{
    static async Task Main(string[] args)
    {
        // 1) Run tests if requested
        if (args.Length > 0 && string.Equals(args[0], "--run-tests", StringComparison.OrdinalIgnoreCase))
        {
            await TestRunner.RunAllAsync();
            return;
        }

        // 2) Compute robust paths relative to exe (bin\Debug\...)
        var exeBase = AppDomain.CurrentDomain.BaseDirectory;
        var webPath = Path.GetFullPath(Path.Combine(exeBase, "..", "..", "..", "web", "summary.json"));
        var sourcesPath = Path.GetFullPath(Path.Combine(exeBase, "..", "..", "..", "sources.json"));

        Console.WriteLine("Exe Base: " + exeBase);
        Console.WriteLine("Web summary.json path: " + webPath);
        Console.WriteLine("Sources.json path: " + sourcesPath);

        try
        {
            // 3) Read sources.json text (if present)
            string sourcesText = File.Exists(sourcesPath) ? File.ReadAllText(sourcesPath) : null;
            bool hasSourcesText = !string.IsNullOrWhiteSpace(sourcesText);

            if (hasSourcesText)
            {
                var sourcesRoot = JsonConvert.DeserializeObject<SourcesRoot>(sourcesText);
                var configs = (sourcesRoot != null && sourcesRoot.sources != null)
                              ? sourcesRoot.sources
                              : new List<SourceConfig>();

                if (configs.Count > 0)
                {
                    Console.WriteLine("Multi-API mode: " + configs.Count + " source(s) configured.");
                    await RunMultiApiFlow(configs, webPath);
                }
                else
                {
                    Console.WriteLine("No sources configured in sources.json. Falling back to single-API.");
                    await RunSingleApiFlow(webPath);
                }
            }
            else
            {
                Console.WriteLine("sources.json missing or empty. Running single-API flow.");
                await RunSingleApiFlow(webPath);
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine("ERROR: " + ex.Message);
            Console.WriteLine("StackTrace: " + ex.StackTrace);
        }
    }

    // ---------------- Multi-API flow ----------------
    private static async Task RunMultiApiFlow(List<SourceConfig> configs, string webPath)
    {
        var adapterPairs = new List<Tuple<IApiSource, SourceConfig>>();
        foreach (var cfg in configs)
        {
            var type = (cfg.type ?? string.Empty).ToLowerInvariant();
            IApiSource adapter = null;

            if (type == "spacex") adapter = new SpaceXSource();
            else if (type == "coingecko") adapter = new CoinGeckoSource();
            else if (type == "openmeteo") adapter = new OpenMeteoSource();

            if (adapter != null)
                adapterPairs.Add(new Tuple<IApiSource, SourceConfig>(adapter, cfg));
            else
                Console.WriteLine("Unsupported source type: '" + cfg.type + "'. Skipping.");
        }

        if (adapterPairs.Count == 0)
        {
            Console.WriteLine("No supported adapters created. Falling back to single-API.");
            await RunSingleApiFlow(webPath);
            return;
        }

        using (var http = new HttpClient { Timeout = TimeSpan.FromSeconds(10) })
        {
            // Set default headers to avoid 403 on some APIs
            http.DefaultRequestHeaders.Accept.Clear();
            http.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            // User-Agent is required by many APIs; format must be valid
            http.DefaultRequestHeaders.UserAgent.Clear();
            http.DefaultRequestHeaders.UserAgent.Add(new ProductInfoHeaderValue("ApiHarvester", "1.0"));

            // Local helper: safe fetch that catches and logs errors
            async Task<ApiResult> TryFetchAsync(IApiSource adapter, SourceConfig cfg)
            {
                try
                {
                    return await adapter.FetchAsync(http, cfg);
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"WARN: Fetch failed for '{cfg.name}' ({cfg.type}): {ex.Message}");
                    return null; // skip this source
                }
            }

            // Start all fetches in parallel, catch per-source errors
            var tasks = adapterPairs.Select(p => TryFetchAsync(p.Item1, p.Item2)).ToArray();
            var results = await Task.WhenAll(tasks);

            // Filter out failed sources
            var okResults = results.Where(r => r != null).ToList();
            if (okResults.Count == 0)
            {
                Console.WriteLine("All sources failed. Falling back to single-API.");
                await RunSingleApiFlow(webPath);
                return;
            }

            // Aggregate and write summary.json
            var summary = MultiSourceProcessor.Aggregate(okResults);
            Directory.CreateDirectory(Path.GetDirectoryName(webPath));
            var json = JsonConvert.SerializeObject(summary, Formatting.Indented);
            File.WriteAllText(webPath, json);
            Console.WriteLine("Multi-source summary.json generated at: " + webPath);
        }
    }

    private static async Task RunSingleApiFlow(string webPath)
    {
        throw new NotImplementedException();
    }
}