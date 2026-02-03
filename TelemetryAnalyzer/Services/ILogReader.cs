using System.Collections.Generic;
using System.Threading.Tasks;
using TelemetryAnalyzer.Models;

namespace TelemetryAnalyzer.Services
{
    /// <summary>
    /// Interface for reading telemetry logs
    /// Following Interface Segregation Principle - single responsibility
    /// </summary>
    public interface ILogReader
    {
        Task<IEnumerable<TelemetryEvent>> ReadLogsAsync(string filePath);
    }
}