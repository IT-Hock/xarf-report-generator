namespace ITHock.XarfReportGenerator.Plugin
{
    public class Report
    {
        public string SourceIpAddress { get; set; } = "";
        public ushort SourcePort { get; set; }

        public string DestinationIpAddress { get; set; } = "";
        public ushort DestinationPort { get; set; }

        public string Username { get; set; } = "";
        public DateTime DateTime { get; set; }

        private string _logEntry = "";

        // TODO: Move this to the EventViewer Plugin!
        public string LogEntry
        {
            get => _logEntry;
            init
            {
                /*
                %%2305	The specified user account has expired. 
                %%2309	The specified account's password has expired. 
                %%2310	Account currently disabled. 
                %%2311	Account logon time restriction violation. 
                %%2312	User not allowed to logon at this computer. 
                %%2313	Unknown user name or bad password.
                */
                _logEntry = value;
                _logEntry = _logEntry.Replace("%%2305", "The specified user account has expired.");
                _logEntry = _logEntry.Replace("%%2309", "The specified account's password has expired.");
                _logEntry = _logEntry.Replace("%%2310", "Account currently disabled. ");
                _logEntry = _logEntry.Replace("%%2311", "Account logon time restriction violation.");
                _logEntry = _logEntry.Replace("%%2312", "User not allowed to logon at this computer. ");
                _logEntry = _logEntry.Replace("%%2313", "Unknown user name or bad password.");
            }
        }

        public override string ToString()
        {
            return
                $"[{DateTime.ToUniversalTime():O}] {SourceIpAddress}:{SourcePort} -> {DestinationIpAddress}:{DestinationPort} | Username: {Username} | Reason: {LogEntry}";
        }
    }
}