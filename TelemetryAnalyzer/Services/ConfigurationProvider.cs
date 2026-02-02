namespace TelemetryAnalyzer.Services
{
    /// <summary>
    /// Singleton pattern: Ensures only one configuration instance exists
    /// This is thread-safe using lazy initialization
    /// </summary>
    public sealed class ConfigurationProvider
    {
        private static readonly Lazy<ConfigurationProvider> _instance =
            new Lazy<ConfigurationProvider>(() => new ConfigurationProvider());

        private readonly object _lock = new object();
        private int _processedCount = 0;

        // Public properties
        public string InputFilePath { get; private set; }
        public string OutputFilePath { get; private set; }
        public int BatchSize { get; private set; }

        // Singleton instance accessor
        public static ConfigurationProvider Instance => _instance.Value;

        // Private constructor prevents external instantiation
        private ConfigurationProvider()
        {
            // Default configuration
            InputFilePath = "Data/sample-logs.csv";
            OutputFilePath = "Output/summary-report.json";
            BatchSize = 1000;
        }

        /// <summary>
        /// Thread-safe counter increment using lock
        /// </summary>
        public void IncrementProcessedCount()
        {
            lock (_lock)
            {
                _processedCount++;
            }
        }

        public int GetProcessedCount()
        {
            lock (_lock)
            {
                return _processedCount;
            }
        }

        public void SetInputFilePath(string path)
        {
            InputFilePath = path;
        }

        public void SetOutputFilePath(string path)
        {
            OutputFilePath = path;
        }
    }
}