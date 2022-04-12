using System.Globalization;
using System.Text.RegularExpressions;
using SimpleLogger;

namespace ITHock.XarfReportGenerator.Plugin.IPBan;

public class IPBanCollector : IReportCollector
{
    private const string IpBanProDefaultLogPath = @"C:\Program Files\IPBanProPersonal";
    private const string IpBanFreeDefaultLogPath = @"C:\Program Files\IPBan";

    private static readonly Regex LineRegex =
        new(@"(?<datetime>\d{4}-\d{2}-\d{2} \d{2}:\d{2}:\d{2}\.\d{4})\|WARN\|IPBan\|(?<message>.*)",
            RegexOptions.Compiled);

    private static readonly Regex MessageRegex =
        new(
            @"Banning ip address: (?<ip>.*), user name: (?<username>.*), config blacklisted: (?<configblacklisted>.*), count: (?<count>.*), extra info: (?<extrainfo>.*), duration: (?<duration>.*)",
            RegexOptions.Compiled);

    private readonly IPBanPlugin? _plugin;

    public IPBanCollector(IPlugin plugin)
    {
        _plugin = (IPBanPlugin?)plugin;
    }

    public IEnumerable<Report> GatherReports()
    {
        if (_plugin == null || !_plugin.IsInitialized)
        {
            Logger.Log(Logger.Level.Error, "[IPBanPlugin] Plugin not initialized");
            return Array.Empty<Report>();
        }

        if (_plugin.Config == null)
        {
            Logger.Log(Logger.Level.Error, "[IPBanPlugin] Plugin configuration not set");
            return Array.Empty<Report>();
        }

        var logDirectory = _plugin.Config.LogDirectory;
        if (!Directory.Exists(logDirectory))
        {
            if (Directory.Exists(IpBanProDefaultLogPath))
            {
                logDirectory = IpBanProDefaultLogPath;
            }
            else if (Directory.Exists(IpBanFreeDefaultLogPath))
            {
                logDirectory = IpBanFreeDefaultLogPath;
            }
            else
            {
                Logger.Log(Logger.Level.Warning, "[IPBanPlugin] No log directory found");
                return Array.Empty<Report>();
            }
        }

        var files = Directory.GetFiles(logDirectory, "*logfile*.txt", SearchOption.AllDirectories);
        if (files.Length == 0)
            return Array.Empty<Report>();

        var reports = new List<Report>();
        foreach (var ipBanLogFile in files)
        {
            var logFileLines = File.ReadAllLines(ipBanLogFile);
            foreach (var logFileLine in logFileLines)
            {
                //Logger.Log(Logger.Level.Debug, $"Processing log file line: {logFileLine}");

                var lineMatch = LineRegex.Match(
                    logFileLine);
                if (!lineMatch.Success)
                {
                    Logger.Log(Logger.Level.Debug, $"Skipping invalid log line: {logFileLine}");
                    continue;
                }
                
                var message = lineMatch.Groups["message"].Value;
                var datetime = DateTime.ParseExact(lineMatch.Groups["datetime"].Value, "yyyy-MM-dd HH:mm:ss.ffff",
                    CultureInfo.CurrentCulture);

                var match2 = MessageRegex.Match(message);
                if (!match2.Success)
                {
                    Logger.Log(Logger.Level.Debug, $"Skipping log entry with invalid message '{message}'");
                    continue;
                }

                var ip = match2.Groups["ip"].Value;
                var username = match2.Groups["username"].Value;
                //var configblacklisted = match2.Groups["configblacklisted"].Value;
                //var count = match2.Groups["count"].Value;
                //var extrainfo = match2.Groups["extrainfo"].Value;
                //var duration = match2.Groups["duration"].Value;

                var report = new Report
                {
                    SourceIpAddress = ip,
                    Username = username,
                    DateTime = datetime,
                    SourcePort = 0, // We do not have this information!
                    LogEntry = logFileLine,
                    DestinationPort = 0, // We do not have this information!
                    DestinationIpAddress = "0.0.0.0" //config.MyIpAddress
                };
                reports.Add(report);
            }
        }

        return reports;
    }
}