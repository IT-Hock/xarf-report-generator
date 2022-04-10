using System.Reflection;
using ITHock.XarfReportGenerator.Plugin;

namespace ITHock.XarfReportGenerator;

internal class Program
{
    private static void Main(string[] args)
    {
        var pluginSystem =
            new PluginSystem(Path.GetFullPath(Path.Combine(Path.GetDirectoryName(typeof(Program).Assembly.Location),
                "Plugins")));

        var reports = new List<Report>();
        foreach (var reportCollector in pluginSystem.GetReportCollectors())
        {
            reports.AddRange(reportCollector.GatherReports());
        }

        Console.WriteLine($"Found {reports.Count} reports");
    }
}