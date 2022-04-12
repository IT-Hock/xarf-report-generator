using System.Reflection;
using ITHock.XarfReportGenerator.Plugin.Utils;
using Newtonsoft.Json;

namespace ITHock.XarfReportGenerator.Plugin.IIS;

public class IISPlugin : IPlugin
{
    public string Name => "IIS";
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
        
        IsInitialized = true;
    }
}