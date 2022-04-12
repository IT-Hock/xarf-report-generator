using System.Net;
using ITHock.XarfReportGenerator.Plugin;
using ITHock.XarfReportGenerator.Utils;
using SimpleLogger;
using SimpleLogger.Logging.Handlers;

namespace ITHock.XarfReportGenerator;

internal class Program
{
    private static async Task Main(string[] args)
    {
        Logger.LoggerHandlerManager
            .AddHandler(new SimpleLoggerConsoleLogger())
            .AddHandler(new FileLoggerHandler(new SimpleLoggerFormat()))
            .AddHandler(new DebugConsoleLoggerHandler(new SimpleLoggerFormat()));

        var cacheDatabase = new CacheDatabase();
        cacheDatabase.Initialize();

        var assemblyDir = Path.GetDirectoryName(typeof(Program).Assembly.Location);
        if (assemblyDir == null)
            throw new Exception("Could not get assembly directory");

        var pluginSystem = new PluginSystem(Path.GetFullPath(Path.Combine(assemblyDir, "Plugins")));

        var reports = new List<Report>();
        foreach (var plugin in pluginSystem.GetReportCollectors())
        {
            if (plugin.ReportCollector == null) throw new Exception("ReportCollector is null");

            var foundReports = plugin.ReportCollector.GatherReports().ToList();
            foreach (var t in foundReports)
            {
                t.Source = plugin.Name;
            }

            reports.AddRange(foundReports);
            Logger.Log(Logger.Level.Debug, $"Found reports {plugin.Name} => {foundReports.Count}");
        }

        Logger.Log(Logger.Level.Info, $"Found {reports.Count} reports");

        foreach (var report in reports)
        {
            // Skip the processing of reports that come from an internal ip
            if (IPAddress.Parse(report.SourceIpAddress).IsInternal()) continue;
            
            // TODO: Maybe make this a config option?
            var cachedIp = await cacheDatabase.GetCachedIp(report.SourceIpAddress) ??
                           await cacheDatabase.AddCachedIp(report.SourceIpAddress);
            report.SourceIpAddressGeography = cachedIp;

            foreach (var plugin in pluginSystem.GetReportProcessors())
            {
                if (plugin.ReportProcessor == null) throw new Exception("ReportCollector is null");

                var result = plugin.ReportProcessor.ProcessReport(report);
                Logger.Log(Logger.Level.Debug,
                    $"Processed report {report.SourceIpAddress} using {plugin.Name} => {result}");
            }
        }

        Logger.Log(Logger.Level.Info, $"Processed {reports.Count} reports");
    }
}