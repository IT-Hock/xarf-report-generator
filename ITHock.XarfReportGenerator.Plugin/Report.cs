using ITHock.XarfReportGenerator.Plugin.Utils;

namespace ITHock.XarfReportGenerator.Plugin
{
    public class Report
    {
        public string SourceIpAddress { get; set; } = "";
        public ushort SourcePort { get; set; }
        
        public IpAddressGeography? SourceIpAddressGeography { get; set; }

        public string DestinationIpAddress { get; set; } = "";
        public ushort DestinationPort { get; set; }

        public string Username { get; set; } = "";
        public DateTime DateTime { get; set; }
        
        public string LogEntry { get; set; }

        public string Source { get; set; }

        public override string ToString()
        {
            return
                $"[{DateTime.ToUniversalTime():O}] {SourceIpAddress}:{SourcePort} -> {DestinationIpAddress}:{DestinationPort} | Username: {Username} | Reason: {LogEntry}";
        }
    }
}