using Newtonsoft.Json;

namespace ITHock.XarfReportGenerator.Plugin.EventViewer;

[JsonObject]
public class Configuration
{
    [JsonProperty("FilterEventID")]
    public List<int> FilterEventId { get; set; } = new();
}