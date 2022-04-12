using System.Globalization;
using System.Text.RegularExpressions;
using ITHock.XarfReportGenerator.Plugin.Utils;
using SimpleLogger;

namespace ITHock.XarfReportGenerator.Plugin.nginx;

public class NginxCollector : IReportCollector
{
    private readonly NginxPlugin? _plugin;

    private readonly Regex _nginxLineRegex =
        new(
            @"(?<ip>\d+\.\d+\.\d+\.\d+)\s.*?\s.*?\s.*?\[(?<datetime>.*?)\]\s""(?<method>.*?)\s(?<url>.*?)\s(?<http>.*?)\s(?<status>\d+)\s(?<sent>\d+)\s"".*?""\s""(?<useragent>.*?)""");

    private readonly Regex _nginxDateRegex = new(@"\d{2}/[a-zA-Z]{3}/\d{4}:\d{2}:\d{2}:\d{2}");

    public NginxCollector(IPlugin plugin)
    {
        _plugin = (NginxPlugin?)plugin;
    }

    public IEnumerable<Report> GatherReports()
    {
        if (_plugin == null || !_plugin.IsInitialized)
        {
            Logger.Log(Logger.Level.Error, "[nginxPlugin] Plugin not initialized");
            return Array.Empty<Report>();
        }

        if (_plugin.Config == null)
        {
            Logger.Log(Logger.Level.Error, "[nginxPlugin] Plugin configuration not set");
            return Array.Empty<Report>();
        }
        
        if (!Directory.Exists(_plugin.Config.LogDirectory))
        {
            Logger.Log(Logger.Level.Warning, $"[nginxPlugin] Log directory '{_plugin.Config.LogDirectory}' does not exist");
            return Array.Empty<Report>();
        }

        var reports = new List<Report>();

        var logFiles = Directory.GetFiles(_plugin.Config.LogDirectory, "*.log", SearchOption.AllDirectories);
        foreach (var logFile in logFiles)
        {
            //var lines = File.ReadAllLines(logFile);
            var lines = PluginUtilities.ReadAllSharedLines(logFile);
            foreach (var line in lines)
            {
                var match = _nginxLineRegex.Match(line);
                if (!match.Success) continue;

                var ip = match.Groups["ip"].Value;
                var dateMatch = _nginxDateRegex.Match(match.Groups["datetime"].Value);
                var date = dateMatch.Groups[0].Value;
                var url = match.Groups["url"].Value;
                var method = match.Groups["method"].Value;
                var status = match.Groups["status"].Value;

                var dateTime = DateTime.ParseExact(date, "dd/MMM/yyyy:HH:mm:ss", CultureInfo.CurrentCulture);

                if (_plugin.Config.MethodFilter != null)
                {
                    var methodRegex = new Regex(_plugin.Config.MethodFilter);
                    if (!methodRegex.IsMatch(method)) continue;
                }

                if (_plugin.Config.StatusCodeFilter != null)
                {
                    var statusCodeRegex = new Regex(_plugin.Config.StatusCodeFilter);
                    if (!statusCodeRegex.IsMatch(status)) continue;
                }

                if (_plugin.Config.PathFilter != null)
                {
                    var pathRegex = new Regex(_plugin.Config.PathFilter);
                    if (!pathRegex.IsMatch(url)) continue;
                }

                reports.Add(new Report
                {
                    DateTime = dateTime,
                    SourceIpAddress = ip,
                    LogEntry = line,
                });
            }
        }

        return reports;
    }
}