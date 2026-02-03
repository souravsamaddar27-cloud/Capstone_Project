using System;
using System.IO;
using System.Text.Json;
using System.Threading.Tasks;
using TelemetryAnalyzer.Models;

namespace TelemetryAnalyzer.Services
{
    /// <summary>
    /// Writes reports to JSON files asynchronously
    /// Single Responsibility: Only handles report writing
    /// </summary>
    public class ReportWriter : IReportWriter
    {
        public async Task WriteReportAsync(SummaryReport report, string outputPath)
        {
            try
            {
                // Ensure output directory exists
                var directory = Path.GetDirectoryName(outputPath);
                if (!string.IsNullOrEmpty(directory) && !Directory.Exists(directory))
                {
                    Directory.CreateDirectory(directory);
                }

                // Serialize to JSON with pretty printing
                var options = new JsonSerializerOptions
                {
                    WriteIndented = true,
                    PropertyNamingPolicy = JsonNamingPolicy.CamelCase
                };

                var json = JsonSerializer.Serialize(report, options);

                // Write asynchronously
                await File.WriteAllTextAsync(outputPath, json);

                Console.WriteLine($"✓ Report written successfully to: {outputPath}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"✗ Error writing report: {ex.Message}");
                throw;
            }
        }
    }
}