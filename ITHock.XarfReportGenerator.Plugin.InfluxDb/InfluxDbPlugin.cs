using System.Reflection;
using InfluxDB.Collector.Diagnostics;
using ITHock.XarfReportGenerator.Plugin.Utils;
using Newtonsoft.Json;

namespace ITHock.XarfReportGenerator.Plugin.InfluxDb;

public class InfluxDbPlugin : IPlugin
{
    public string Name => "InflxuDB";
    public string Author => "IT-Hock";

    public Configuration? Config { get; set; }

    internal bool IsInitialized;

    public void Initialize()
    {
        Config = PluginUtilities.GetConfig<Configuration>();
        if (Config == null)
        {
            PluginUtilities.SaveConfig(new Configuration());
            throw new Exception("Could not deserialize config");
        }

        CollectorLog.RegisterErrorHandler((message, exception) => { Console.WriteLine($"{message}: {exception}"); });
        IsInitialized = true;
    }
}