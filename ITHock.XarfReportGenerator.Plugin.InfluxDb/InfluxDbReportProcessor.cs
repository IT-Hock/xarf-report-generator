using InfluxDB.Collector;

namespace ITHock.XarfReportGenerator.Plugin.InfluxDb;

public class InfluxDbReportProcessor : IReportProcessor
{
    private readonly InfluxDbPlugin _plugin;

    public InfluxDbReportProcessor(IPlugin plugin)
    {
        _plugin = (InfluxDbPlugin)plugin;
    }

    public bool ProcessReport(Report report)
    {
        if(!_plugin.IsInitialized)
            return false;
        
        var config = _plugin.Config;
        if(config == null)
            return false;
        
        Metrics.Collector = new CollectorConfiguration()
            .Tag.With("host", Environment.GetEnvironmentVariable("COMPUTERNAME"))
            .WriteTo.InfluxDB(config.InfluxUrl, config.InfluxDbName, config.InfluxDbUser, config.InfluxDbPassword)
            .CreateCollector();

        Metrics.Collector.Write("report", new Dictionary<string, object>
        {
            { "IPAddress", report.SourceIpAddress },
            { "Source", report.Source },
            { "LogEntry", report.LogEntry }
        }, null, report.DateTime.ToUniversalTime());
        Metrics.Close();

        return true;
    }
}