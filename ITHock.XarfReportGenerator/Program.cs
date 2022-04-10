using System.Reflection;
using ITHock.XarfReportGenerator.Plugin;
using ITHock.XarfReportGenerator.Utils;
using SimpleLogger;
using SimpleLogger.Logging.Handlers;

namespace ITHock.XarfReportGenerator;

internal class Program
{
    private static void Main(string[] args)
    {
        Logger.LoggerHandlerManager
            .AddHandler(new SimpleLoggerConsoleLogger())
            .AddHandler(new FileLoggerHandler(new SimpleLoggerFormat()))
            .AddHandler(new DebugConsoleLoggerHandler(new SimpleLoggerFormat()));
        
        var pluginSystem =
            new PluginSystem(Path.GetFullPath(Path.Combine(Path.GetDirectoryName(typeof(Program).Assembly.Location),
                "Plugins")));

        var reports = new List<Report>();
        foreach (var plugin in pluginSystem.GetReportCollectors())
        {
            var foundReports = plugin.ReportCollector.GatherReports();
            reports.AddRange(foundReports);
            Logger.Log(Logger.Level.Debug, $"Found reports {plugin.Name} => {foundReports.Count()}"); 
        }
        Logger.Log(Logger.Level.Info, $"Found {reports.Count} reports");

        foreach (var report in reports)
        {
            foreach (var plugin in pluginSystem.GetReportProcessors())
            {
                var result = plugin.ReportProcessor.ProcessReport(report);
                Logger.Log(Logger.Level.Debug, $"Processed report {report.SourceIpAddress} using {plugin.Name} => {result}"); 
            }
        }
        
        Logger.Log(Logger.Level.Info, $"Processed {reports.Count} reports");

    }
}