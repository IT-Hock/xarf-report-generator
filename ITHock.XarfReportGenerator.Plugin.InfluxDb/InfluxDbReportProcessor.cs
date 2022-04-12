using InfluxDB.Collector;
using SimpleLogger;

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
        if (!_plugin.IsInitialized)
        {
            Logger.Log(Logger.Level.Error, "[InfluxDBPlugin] Plugin is not initialized");
            return false;
        }

        var config = _plugin.Config;
        if (config == null)
        {
            Logger.Log(Logger.Level.Error, "[InfluxDBPlugin] Plugin config is null");
            return false;
        }

        Metrics.Collector = new CollectorConfiguration()
            .Tag.With("host", Environment.GetEnvironmentVariable("COMPUTERNAME"))
            .WriteTo.InfluxDB(config.InfluxUrl, config.InfluxDbName, config.InfluxDbUser, config.InfluxDbPassword)
            .CreateCollector();

        Metrics.Collector.Write("report", new Dictionary<string, object>
        {
            { "IPAddress", report.SourceIpAddress },
            { "Source", report.Source },
            { "LogEntry", report.LogEntry },
            { "IP_Country", report.SourceIpAddressGeography?.Geography.CountryCode },
            { "IP_ISP", report.SourceIpAddressGeography?.Geography.ISP },
            { "IP_AbuseMail", report.SourceIpAddressGeography?.AbuseEmail },
        }, null, report.DateTime.ToUniversalTime());
        Metrics.Close();

        return true;
    }
}