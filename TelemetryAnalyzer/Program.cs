using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using TelemetryAnalyzer.Services;

namespace TelemetryAnalyzer
{
    class Program
    {
        static async Task Main(string[] args)
        {
             Console.WriteLine("╔════════════════════════════════════════════════════════╗");
            Console.WriteLine("║     Medtronic Telemetry Log Analyzer v2.0             ║");
            Console.WriteLine("║     SmartSync Capstone Project                         ║");
            Console.WriteLine("╚════════════════════════════════════════════════════════╝");
            Console.WriteLine();

            var stopwatch = Stopwatch.StartNew();

            try
            {
                // Get configuration from Singleton
                var config = ConfigurationProvider.Instance;

                // Generate sample data if it doesn't exist
                if (!File.Exists(config.InputFilePath))
                {
                    Console.WriteLine("📝 Generating sample telemetry dataset (50,000 records)...\n");
                    DataGenerator.GenerateSampleLogs(config.InputFilePath, 50000);
                }

                // Dependency Injection - Manual (Constructor Injection)
                ILogReader logReader = new LogReader();
                IAggregator aggregator = new Aggregator();
                IReportWriter reportWriter = new ReportWriter();

                Console.WriteLine($"📁 Reading logs from: {config.InputFilePath}");
                Console.WriteLine("⏳ Processing telemetry data asynchronously...\n");

                // Step 1: Read logs asynchronously
                var events = await logReader.ReadLogsAsync(config.InputFilePath);
                Console.WriteLine($"✓ Loaded {config.GetProcessedCount():N0} telemetry events");

                // Step 2: Aggregate data using LINQ
                Console.WriteLine("📊 Aggregating data with LINQ queries...");
                var report = aggregator.AggregateData(events);
                Console.WriteLine($"✓ Analysis complete in {report.ProcessingTimeMs:F2}ms");

                // Step 3: Write report asynchronously
                Console.WriteLine($"💾 Writing JSON summary report...");
                await reportWriter.WriteReportAsync(report, config.OutputFilePath);

                // Copy to web folder for dashboard
                await CopyToWebFolder(config.OutputFilePath);

                stopwatch.Stop();

                // Display summary
                Console.WriteLine("\n╔════════════════════════════════════════════════════════╗");
                Console.WriteLine("║                   ANALYSIS SUMMARY                     ║");
                Console.WriteLine("╚════════════════════════════════════════════════════════╝");
                Console.WriteLine($"Total Events Processed: {report.TotalEvents:N0}");
                Console.WriteLine($"Unique Devices: {report.DeviceStatistics.Count}");
                Console.WriteLine($"Total Processing Time: {stopwatch.ElapsedMilliseconds}ms");
                Console.WriteLine($"JSON Output Location: {config.OutputFilePath}");

                // Display key insights
                Console.WriteLine("\n📊 Key Insights:");
                Console.WriteLine($"   • Critical Errors: {(report.SeverityDistribution.ContainsKey("Critical") ? report.SeverityDistribution["Critical"] : 0):N0}");
                Console.WriteLine($"   • High Severity Events: {(report.SeverityDistribution.ContainsKey("High") ? report.SeverityDistribution["High"] : 0):N0}");
                Console.WriteLine($"   • Top Error Type: {(report.TopErrors.Count > 0 ? report.TopErrors.First().Key : "None")}");

                Console.WriteLine($"\n✓ Analysis complete! Open web/index.html to view the dashboard.");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"\n✗ ERROR: {ex.Message}");
                Console.WriteLine($"Stack Trace: {ex.StackTrace}");
            }

            Console.WriteLine("\nPress any key to exit...");
            Console.ReadKey();
        }

        static async Task CopyToWebFolder(string sourcePath)
        {
            try
            {
                string[] possibleWebPaths = { "web", "Web", "../web", "../Web", "../../web", "../../Web", "../../../web", "../../../Web" };
                string webFolder = null;

                foreach (var path in possibleWebPaths)
                {
                    if (Directory.Exists(path))
                    {
                        webFolder = path;
                        break;
                    }
                }

                if (webFolder != null)
                {
                    var webReportPath = Path.Combine(webFolder, "summary-report.json");
                    File.Copy(sourcePath, webReportPath, true);
                    Console.WriteLine($"✓ Dashboard JSON file copied to web folder");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"⚠ Could not copy to web folder: {ex.Message}");
            }

            await Task.CompletedTask;
        }
    }
}