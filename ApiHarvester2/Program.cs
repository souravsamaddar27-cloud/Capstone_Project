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
using Metrics.Core.Enums;
using Metrics.Core.Interfaces;
using Metrics.Persistence.Db;
using Metrics.Persistence.Services;
using Microsoft.EntityFrameworkCore;
using ApiHarvester.Services;


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

        var projectRoot = Path.GetFullPath(
            Path.Combine(exeBase, "..", "..", "..", ".."));

        var webPath = Path.Combine(projectRoot, "web", "summary.json");

        var sourcesPath = Path.Combine(projectRoot, "sources.json");


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
    private static async Task RunMultiApiFlow(
     List<SourceConfig> configs,
     string webPath)
    {
        var options = new DbContextOptionsBuilder<MetricsDbContext>()
            .UseNpgsql(
    "Host=ep-lively-feather-a8pmfu4c.eastus2.azure.neon.tech;" +
    "Port=5432;" +
    "Database=neondb;" +
    "Username=neondb;" +
    "Password=npg_ZW6jz5KGQmYs;" +
    "SSL Mode=Require;" +
    "Trust Server Certificate=true"
)
            .Options;

        using var db = new MetricsDbContext(options);
        IMetricsRecorder recorder = new MetricsRecorder(db);

        using var http = new HttpClient
        {
            Timeout = TimeSpan.FromSeconds(10)
        };

        // =====================
        // 1️⃣ FETCH
        // =====================

        var adapters = new List<(IApiSource adapter, SourceConfig config)>();

        foreach (var cfg in configs)
        {
            var type = (cfg.type ?? "").ToLowerInvariant();

            IApiSource? adapter = null;

            if (type == "spacex") adapter = new SpaceXSource();
            else if (type == "coingecko") adapter = new CoinGeckoSource();
            else if (type == "openmeteo") adapter = new OpenMeteoSource();

            if (adapter != null)
                adapters.Add((adapter, cfg));
        }

        var results = await recorder.MeasureAsync<ApiResult[]>(
            ActivityType.Api,
            "Fetch_All",
            async () =>
            {
                var tasks = adapters
                    .Select(pair => pair.adapter.FetchAsync(http, pair.config))
                    .ToArray();

                return await Task.WhenAll(tasks);
            });

        // =====================
        // 2️⃣ PROCESS
        // =====================

        var summary = await recorder.MeasureAsync<SummaryReport>(
            ActivityType.Api,
            "Process_All",
            async () =>
            {
                var validResults = results
                    .Where(r => r != null)
                    .ToList();

                return MultiSourceProcessor.Aggregate(validResults);
            });

        // =====================
        // 3️⃣ REPORT WRITE
        // =====================

        await recorder.MeasureAsync<bool>(
            ActivityType.Api,
            "Report_Write",
            async () =>
            {
                Directory.CreateDirectory(Path.GetDirectoryName(webPath)!);

                var json = JsonConvert.SerializeObject(summary, Formatting.Indented);
                File.WriteAllText(webPath, json);

                return true;
            });
    }




    private static async Task RunSingleApiFlow(string webPath)
    {
        Console.WriteLine("Single API flow not implemented yet.");
        await Task.CompletedTask;
    }

}

 