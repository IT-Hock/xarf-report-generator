using ITHock.XarfReportGenerator.Plugin.Utils;

namespace ITHock.XarfReportGenerator.Plugin.EventViewer;

public class EventViewerPlugin : IPlugin
{
    public string Name => "EventViewer";

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