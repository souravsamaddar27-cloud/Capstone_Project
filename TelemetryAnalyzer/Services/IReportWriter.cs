using System.Threading.Tasks;
using TelemetryAnalyzer.Models;

namespace TelemetryAnalyzer.Services
{
    /// <summary>
    /// Interface for writing reports
    /// </summary>
    public interface IReportWriter
    {
        Task WriteReportAsync(SummaryReport report, string outputPath);
    }
}