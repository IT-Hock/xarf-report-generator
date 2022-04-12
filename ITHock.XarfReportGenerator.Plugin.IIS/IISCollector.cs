using System.Text.RegularExpressions;
using SimpleLogger;
using Tx.Windows;

namespace ITHock.XarfReportGenerator.Plugin.IIS;

public class IISCollector : IReportCollector
{
    private readonly IISPlugin? _plugin;
    private static readonly string _defaultLogPath = @"C:\inetpub\logs";

    public IISCollector(IPlugin plugin)
    {
        _plugin = (IISPlugin?)plugin;
    }

    public IEnumerable<Report> GatherReports()
    {
        if (_plugin == null || !_plugin.IsInitialized)
            return Array.Empty<Report>();
        if (_plugin.Config == null)
            return Array.Empty<Report>();
        
        var reports = new List<Report>();
        var logFiles = Directory.GetFiles(_defaultLogPath, "*.log", SearchOption.AllDirectories);
        foreach (var logFile in logFiles)
        {
            var logEntries = W3CEnumerable.FromFile(logFile);
            foreach (var logEntry in logEntries)
            {
                var query = (logEntry.cs_uri_query != "" ? "?" + logEntry.cs_uri_query : "-");
                var uri =
                    $"{logEntry.c_ip} {logEntry.cs_username} {query} [{logEntry.dateTime:O}] \"{logEntry.cs_method} {logEntry.cs_uri_stem} HTTP/{logEntry.cs_version}\" {logEntry.sc_status} {logEntry.sc_bytes} \"{logEntry.cs_Referer}\" \"{logEntry.cs_User_Agent}\"";

                if (_plugin.Config.MethodFilter != null)
                {
                    var methodRegex = new Regex(_plugin.Config.MethodFilter);
                    if (logEntry.cs_method == null) continue;
                    
                    if (!methodRegex.IsMatch(logEntry.cs_method))
                    {
                        Logger.Log(Logger.Level.Debug,$"[IISPlugin] {logEntry.cs_method} does not match {_plugin.Config.MethodFilter}");
                        continue;
                    }
                }

                if (_plugin.Config.StatusCodeFilter != null)
                {
                    var statusCodeRegex = new Regex(_plugin.Config.StatusCodeFilter);
                    if (logEntry.sc_status == null) continue;
                    
                    if (!statusCodeRegex.IsMatch(logEntry.sc_status))
                    {
                        Logger.Log(Logger.Level.Debug,$"[IISPlugin] {logEntry.sc_status} does not match {_plugin.Config.StatusCodeFilter}");
                        continue;
                    }
                }

                if (_plugin.Config.PathFilter != null)
                {
                    var pathRegex = new Regex(_plugin.Config.PathFilter);
                    if (logEntry.cs_uri_stem == null) continue;
                    
                    if (!pathRegex.IsMatch(logEntry.cs_uri_stem))
                    {
                        Logger.Log(Logger.Level.Debug,$"[IISPlugin] {logEntry.cs_uri_stem} did not match {_plugin.Config.PathFilter}");
                        continue;
                    }
                }

                var report = new Report
                {
                    DateTime = logEntry.dateTime,
                    SourceIpAddress = logEntry.c_ip,
                    Username = logEntry.cs_username,
                    LogEntry = uri,
                    DestinationPort = ushort.Parse(logEntry.s_port)
                };
                reports.Add(report);
            }
        }
        
        return reports;
    }
}