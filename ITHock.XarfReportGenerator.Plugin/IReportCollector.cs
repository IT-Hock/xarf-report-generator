namespace ITHock.XarfReportGenerator.Plugin;

public interface IReportCollector
{
    IEnumerable<Report> GatherReports();
}