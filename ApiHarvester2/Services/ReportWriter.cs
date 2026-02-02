using Newtonsoft.Json;
using ApiHarvester.Models;

namespace ApiHarvester.Services
{
    public class ReportWriter : IReportWriter
    {
        public void WriteSummary(SummaryReport summary, string filePath)
        {
            var json = JsonConvert.SerializeObject(summary, Formatting.Indented);
            System.IO.File.WriteAllText(filePath, json);
        }
    }
}