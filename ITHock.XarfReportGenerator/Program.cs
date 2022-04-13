using System.Net;
using CommandLine;
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
#if !DEBUG
        Logger.DefaultLevel = Logger.Level.Info;
        Logger.DebugOff();
#endif

        var arguments = Parser.Default.ParseArguments<CommandLineOptions>(args);
        if (arguments.Errors.Any())
        {
            if (arguments.Errors.IsVersion())
            {
                Environment.Exit(0);
                return;
            }

            if (!arguments.Errors.IsHelp() && !arguments.Errors.IsVersion() &&
                arguments.Errors.All(x => x.Tag != ErrorType.UnknownOptionError))
            {
                Logger.Log(Logger.Level.Error, "Error parsing command line arguments.");
                Logger.Log(Logger.Level.Error, string.Join(Environment.NewLine, arguments.Errors));
            }

            Environment.Exit(0);
        }

        var cacheDatabase = new CacheDatabase();
        cacheDatabase.Initialize();

        var ipFilter = LoadIpFilter(arguments.Value.Filter);

        var assemblyDir = Path.GetDirectoryName(typeof(Program).Assembly.Location);
        if (assemblyDir == null)
            throw new Exception("Could not get assembly directory");

        var pluginSystem = new PluginSystem(Path.GetFullPath(Path.Combine(assemblyDir, "Plugins")));

        var publicIp = IpAddressExtensions.GetPublicIp();
        Logger.Log(Logger.Level.Debug, $"Got Public IP: {publicIp}");

        Logger.Log(Logger.Level.Info, "Gathering reports from plugins");

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

        Logger.Log(Logger.Level.Info, "Processing reports from plugins");
        
        foreach (var report in reports)
        {
            // Skip the processing of reports that come from an internal ip
            if (IPAddress.Parse(report.SourceIpAddress).IsInternal()) continue;

            if (ipFilter != null)
            {
                if (!ipFilter.Contains(report.SourceIpAddress))
                {
                    Logger.Log(Logger.Level.Debug, $"Skipping report from {report.SourceIpAddress}");
                    continue;
                }
            }

            if (arguments.Value.StartDate != null)
            {
                if(report.DateTime < arguments.Value.StartDate)
                {
                    Logger.Log(Logger.Level.Debug, $"Skipping report from {report.SourceIpAddress} because it is older than {arguments.Value.StartDate}");
                    continue;
                }
            }
            
            if (arguments.Value.EndDate != null)
            {
                if (report.DateTime > arguments.Value.EndDate)
                {
                    Logger.Log(Logger.Level.Debug, $"Skipping report from {report.SourceIpAddress} because it is newer than {arguments.Value.EndDate}");
                    continue;
                }
            }

            // TODO: Maybe make this a config option?
            var cachedIp = await cacheDatabase.GetCachedIp(report.SourceIpAddress) ??
                           await cacheDatabase.AddCachedIp(report.SourceIpAddress);
            report.SourceIpAddressGeography = cachedIp;
            report.DestinationIpAddress = publicIp;

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

    private static List<string>? LoadIpFilter(string? ipFilterFile)
    {
        if (ipFilterFile == null) return null;

        Logger.Log(Logger.Level.Info, $"Loading IP filter '{ipFilterFile}'");

        List<string>? ipFilter;
        var validIp = IPAddress.TryParse(ipFilterFile, out _);
        switch (validIp)
        {
            case false when !File.Exists(ipFilterFile):
                Logger.Log(Logger.Level.Error, $"Filter '{ipFilterFile}' is not a valid IP address or file");
                Environment.Exit(1);
                return null;
            case true:
                ipFilter = new List<string>
                {
                    ipFilterFile
                };
                break;
            default:
            {
                var ìpFilterFile = File.ReadAllLines(ipFilterFile);
                ipFilter = ìpFilterFile.Where(ip => IPAddress.TryParse(ip, out _)).ToList();
                break;
            }
        }

        Logger.Log(Logger.Level.Info, $"{ipFilter.Count} IPs loaded from filter");

        return ipFilter;
    }
}