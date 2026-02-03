using System;
using System.Diagnostics;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace ApiHarvester.Services
{
    public static class MetricsClient
    {
        private static readonly HttpClient _http = new HttpClient();

        public static async Task MeasureAsync(
            string activityName,
            Func<Task> action)
        {
            // Force GC before measuring (optional but cleaner measurement)
            GC.Collect();
            GC.WaitForPendingFinalizers();
            GC.Collect();

            long startMemory = GC.GetTotalMemory(true);
            var stopwatch = Stopwatch.StartNew();

            await action();

            stopwatch.Stop();
            long endMemory = GC.GetTotalMemory(false);

            long executionTime = (long)stopwatch.Elapsed.TotalMilliseconds;
            long memoryUsed = Math.Max(0, endMemory - startMemory);

            await SendMetricAsync(activityName, executionTime, memoryUsed);
        }

        private static async Task SendMetricAsync(
            string activityName,
            long executionTime,
            long memoryBytes)
        {
            var payload = new
            {
                activityType = 1, // API
                subActivityName = activityName,
                executionTimeMs = executionTime,
                memoryBytes = memoryBytes
            };

            var json = JsonConvert.SerializeObject(payload);

            var content = new StringContent(
                json,
                Encoding.UTF8,
                "application/json");

            await _http.PostAsync(
                "http://localhost:5155/api/metrics",
                content);
        }
    }
}
