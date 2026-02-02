using ApiHarvester.Models;

namespace ApiHarvester.Services
{
    public interface IReportWriter
    {
        void WriteSummary(SummaryReport summary, string filePath);
    }
}
