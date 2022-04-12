using Newtonsoft.Json;

namespace ITHock.XarfReportGenerator.Plugin.nginx;

[JsonObject]
public class Configuration
{
    [JsonProperty("StatusCodeFilter")]
    public string? StatusCodeFilter { get; set; } = "403|405|406|400";
    
    [JsonProperty("MethodFilter")]
    public string? MethodFilter { get; set; } = "POST|PUT|DELETE|HEAD|OPTIONS|TRACE|CONNECT";
    
    [JsonProperty("PathFilter")]
    public string? PathFilter { get; set; }
    
    [JsonProperty("LogDirectory")]
    public string LogDirectory { get; set; } = "C:\\Program Files\\nginx\\logs";
}