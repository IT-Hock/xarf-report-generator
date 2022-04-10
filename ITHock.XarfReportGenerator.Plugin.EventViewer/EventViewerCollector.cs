using System.Diagnostics.Eventing.Reader;
using System.Net;

namespace ITHock.XarfReportGenerator.Plugin.EventViewer;

#pragma warning disable CA1416

public class EventViewerCollector : IReportCollector
{
    public IEnumerable<Report> GatherReports()
    {
        var reports = new List<Report>();

        var eventsQuery = new EventLogQuery("Security", PathType.LogName,
            "*[System/EventID=4625 or System/EventID=5152 or System/EventID=4653]");

        try
        {
            var logReader = new EventLogReader(eventsQuery);
            // ReSharper disable once ConditionIsAlwaysTrueOrFalse
            for (var eventRecord = logReader.ReadEvent(); eventRecord != null; eventRecord = logReader.ReadEvent())
            {
                if (eventRecord.Properties.Count < 19)
                {
                    //Logger.Log(Logger.Level.Debug, "Event record has less than 19 properties. Skipping.");
                    continue;
                }

                if (!eventRecord.TimeCreated.HasValue)
                {
                    //Logger.Log(Logger.Level.Debug, "Event record has no time created. Skipping.");
                    continue;
                }

                var username = eventRecord.Properties[5].Value.ToString();
                if (string.IsNullOrEmpty(username))
                {
                    //Logger.Log(Logger.Level.Debug, "Event record has no username. Skipping.");
                    continue;
                }

                var datetime = eventRecord.TimeCreated.Value;

                var ip = eventRecord.Properties[19].Value.ToString();
                if (string.IsNullOrEmpty(ip) || !IPAddress.TryParse(ip, out _))
                {
                    if (ip is "-")
                        continue;

                    //Logger.Log(Logger.Level.Debug, "Event record has no or invalid IP. Skipping.");
                    continue;
                }

                ushort port = 0;
                if (eventRecord.Properties.Count > 20)
                    if (!ushort.TryParse(eventRecord.Properties[20].Value.ToString(), out port))
                    {
                        //Logger.Log(Logger.Level.Debug, "Event record has no or invalid port. Skipping.");
                        continue;
                    }

                var logEntry = eventRecord.Properties[8].Value.ToString() ?? "";
                logEntry = logEntry.Replace("%%2305", "The specified user account has expired.");
                logEntry = logEntry.Replace("%%2309", "The specified account's password has expired.");
                logEntry = logEntry.Replace("%%2310", "Account currently disabled. ");
                logEntry = logEntry.Replace("%%2311", "Account logon time restriction violation.");
                logEntry = logEntry.Replace("%%2312", "User not allowed to logon at this computer. ");
                logEntry = logEntry.Replace("%%2313", "Unknown user name or bad password.");

                reports.Add(new Report
                {
                    SourceIpAddress = ip,
                    SourcePort = 0,
                    DestinationIpAddress = "0.0.0.0", //config.MyIpAddress, 
                    DestinationPort = port,
                    Username = username,
                    DateTime = datetime,
                    LogEntry = logEntry
                });
            }
        }
        catch (EventLogNotFoundException)
        {
            Console.WriteLine("Error while reading the event logs");
            return Array.Empty<Report>();
        }

        return reports;
    }
}

#pragma warning restore CA1416