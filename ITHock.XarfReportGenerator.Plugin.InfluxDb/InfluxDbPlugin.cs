using System.Reflection;
using InfluxDB.Collector.Diagnostics;
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
        var mainAssembly = Assembly.GetAssembly(typeof(InfluxDbPlugin));
        if (mainAssembly == null)
            throw new Exception("Could not find main assembly");

        var assemblyDirectory = Path.GetDirectoryName(mainAssembly.Location);
        if (assemblyDirectory == null)
            throw new Exception("Could not find assembly directory");

        var configPath = Path.Combine(assemblyDirectory, "config.json");
        if (!File.Exists(configPath))
        {
            File.WriteAllText(configPath, JsonConvert.SerializeObject(new Configuration(), Formatting.Indented));
            throw new Exception("Could not find config file");
        }

        var configContent = File.ReadAllText(configPath);
        if (string.IsNullOrEmpty(configContent))
            throw new Exception("Config file is empty");
        
        Config = JsonConvert.DeserializeObject<Configuration>(configContent);
        if (Config == null)
            throw new Exception("Could not deserialize config");

        CollectorLog.RegisterErrorHandler((message, exception) => { Console.WriteLine($"{message}: {exception}"); });
        IsInitialized = true;
    }
}