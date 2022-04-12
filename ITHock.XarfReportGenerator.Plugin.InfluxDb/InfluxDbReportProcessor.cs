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
            .Tag.With("source", report.Source)
            .Tag.With("source_isp", report.SourceIpAddressGeography?.Geography.ISP)
            .Tag.With("source_country", report.SourceIpAddressGeography?.Geography.Country)
            .Tag.With("source_ip", report.SourceIpAddress)
            .Tag.With("source_port", report.SourcePort.ToString())
            .Tag.With("destination", report.DestinationIpAddress)
            .Tag.With("destination_port", report.DestinationPort.ToString())
            .WriteTo.InfluxDB(config.InfluxUrl, config.InfluxDbName, config.InfluxDbUser, config.InfluxDbPassword)
            .CreateCollector();

        Logger.Log(Logger.Level.Debug,$"[InfluxDBPlugin] Sending report {report.DateTime.ToUniversalTime():O} '{report.SourceIpAddress}' to InfluxDB");

        Metrics.Collector.Write("report", new Dictionary<string, object>
        {
            { "IPAddress", report.SourceIpAddress },
            { "Source", report.Source },
            { "LogEntry", report.LogEntry },
            { "IP_Country", report.SourceIpAddressGeography?.Geography.CountryCode },
            { "IP_ISP", report.SourceIpAddressGeography?.Geography.ISP },
            { "IP_AbuseMail", report.SourceIpAddressGeography?.AbuseEmail },
        }, new Dictionary<string, string>()
        {
            {"host", Environment.GetEnvironmentVariable("COMPUTERNAME") },
            {"source", report.Source },
            {"source_ip", report.SourceIpAddress },
            {"source_isp", report.SourceIpAddressGeography?.Geography.ISP },
            {"source_country", report.SourceIpAddressGeography?.Geography.Country },
            {"source_port", report.SourcePort.ToString() },
            {"destination", report.DestinationIpAddress },
            {"destination_port", report.DestinationPort.ToString() },
        }, report.DateTime.ToUniversalTime());
        Metrics.Close();

        return true;
    }
}