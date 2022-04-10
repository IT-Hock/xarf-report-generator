using Newtonsoft.Json;

namespace ITHock.XarfReportGenerator.Plugin.XARF;

public class XARFPlugin : IPlugin
{
    public string Name => "XARF";
    public string Author => "IT-Hock";
    
    public Configuration Config { get; set; }
    
    public void Initialize()
    {
        var configContent = File.ReadAllText("Plugins/XARF/config.json");
        Config = JsonConvert.DeserializeObject<Configuration>(configContent);
    }

    [JsonObject]
    public class Configuration
    {
        [JsonProperty("Organization")]
        public string Organization { get; set; }
        
        [JsonProperty("ContactPhone")]
        public string ContactPhone { get; set; }

        [JsonProperty("ContactName")]
        public string ContactName { get; set; }

        [JsonProperty("ContactEmail")]
        public string ContactEmail { get; set; }

        [JsonProperty("OrganizationEmail")]
        public string OrganizationEmail { get; set; }

        [JsonProperty("Domain")]
        public string Domain { get; set; }

        [JsonProperty("OutputDirectory")]
        public string OutputDirectory { get; set; } = "xarf";
    }
}