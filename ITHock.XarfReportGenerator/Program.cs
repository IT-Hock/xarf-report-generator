using System.Data.SQLite;
using System.Net;
using System.Reflection;
using CommandLine;
using ITHock.XarfReportGenerator;
using ITHock.XarfReportGenerator.Plugins;
using ITHock.XarfReportGenerator.Ripe;
using SimpleLogger;
using SimpleLogger.Logging.Handlers;

#pragma warning disable CS8625

Logger.LoggerHandlerManager
    .AddHandler(new ConsoleLoggerHandler())
    .AddHandler(new FileLoggerHandler())
    .AddHandler(new DebugConsoleLoggerHandler());

var arguments = Parser.Default.ParseArguments<CommandLineOptions>(args);
if (arguments.Errors.Any())
{
    if (arguments.Errors.IsVersion())
    {
        Environment.Exit(0);
        return;
    }

    if (!arguments.Errors.IsHelp() && !arguments.Errors.IsVersion() &&
        arguments.Errors.All(x => x.Tag != ErrorType.UnknownOptionError))
    {
        Logger.Log(Logger.Level.Error, "Error parsing command line arguments.");
        Logger.Log(Logger.Level.Error, string.Join(Environment.NewLine, arguments.Errors));
    }

    Environment.Exit(0);
}

if (!File.Exists(arguments.Value.Config))
{
    Logger.Log($"Configuration file '{arguments.Value.Config}' not found!");
    Environment.Exit(1);
}

if (!Directory.Exists(arguments.Value.Output))
{
    Directory.CreateDirectory(arguments.Value.Output);
}
else if (Directory.EnumerateFiles(arguments.Value.Output, "*").Any())
{
    Logger.Log($"output directory '{arguments.Value.Output}' is not empty");
    Environment.Exit(1);
}

List<string>? ipFilter = null;
if (arguments.Value.Filter != null)
{
    var validIp = IPAddress.TryParse(arguments.Value.Filter, out _);
    if (!validIp && !File.Exists(arguments.Value.Filter))
    {
        Logger.Log($"Filter '{arguments.Value.Filter}' is not a valid IP address or file");
        Environment.Exit(1);
    }

    if (validIp)
    {
        ipFilter = new List<string>
        {
            arguments.Value.Filter
        };
    }
    else
    {
        var ìpFilterFile = File.ReadAllLines(arguments.Value.Filter);
        ipFilter = ìpFilterFile.Where(ip => IPAddress.TryParse(ip, out _)).ToList();
    }
}

var config = Config.Load("config.json");
if (config == null)
{
    Logger.Log("Failed to load config");
    Environment.Exit(1);
}

await using var con =
    new SQLiteConnection(
        $"Data Source={Path.GetDirectoryName(Assembly.GetEntryAssembly()?.Location)}\\ip.db;Version=3;");
con.Open();

{
    await using var cmd = new SQLiteCommand(con);
    cmd.CommandText = @"CREATE TABLE IF NOT EXISTS ip(
            id INTEGER PRIMARY KEY,
            ip TEXT,
            country TEXT,
            countryCode TEXT,
            continent TEXT,
            continentCode TEXT,
            latitude REAL,
            longitude REAL,
            locationAccuracyRadius INTEGER,
            isp TEXT,
            abuseEmail TEXT
    )";
    cmd.ExecuteNonQuery();
}

var reports = new List<Report>();

Logger.Log("Loading plugins...");
if (config.EventViewer.Enabled)
{
    Logger.Log("Fetching failed logons from event viewer...");
    var evtViewer = EventViewer.GetFailedLogons(config).ToList();
    reports.AddRange(evtViewer);
    Logger.Log($"Fetched {evtViewer.Count()} failed logons from EventViewer");
}

if (config.IpBan.Enabled)
{
    Logger.Log("Fetching recent bans from IPBan...");
    var fetchedBans = IpBan.GetRecentBans(config).ToList();
    reports.AddRange(fetchedBans);
    Logger.Log($"Fetched {fetchedBans.Count} recent bans from IPBan");
}

if (ipFilter != null) reports = reports.Where(logon => ipFilter.Contains(logon.SourceIpAddress)).ToList();

if (arguments.Value.StartDate.HasValue)
{
    Logger.Log(Logger.Level.Info, $"Filtering logons before {arguments.Value.StartDate.Value:yyyy-MM-dd}");
    reports = reports.Where(logon => logon.DateTime <= arguments.Value.StartDate.Value).ToList();
}

if (arguments.Value.EndDate.HasValue)
{
    Logger.Log(Logger.Level.Info, $"Filtering logons after {arguments.Value.EndDate.Value:yyyy-MM-dd}");
    reports = reports.Where(logon => logon.DateTime >= arguments.Value.EndDate.Value).ToList();
}

var xarfReporter = new Xarf(config);
var xarfReportFiles = 0;
foreach (var report in reports)
{
    IpAddressGeography? ipAddressGeography = null;

    await using var cmd = new SQLiteCommand("SELECT * FROM ip WHERE ip=@ip", con);
    cmd.Parameters.AddWithValue("@ip", report.SourceIpAddress);
    await using var rdr = cmd.ExecuteReader();
    if (!rdr.HasRows)
    {
        ipAddressGeography = await IpBan.LookupIp(report.SourceIpAddress);
        var abuseContactEmail = await Ripe.QueryAbuseContact(report.SourceIpAddress);
        if (ipAddressGeography != null && !ipAddressGeography.Error)
        {
            await using var insertCommand = new SQLiteCommand(con);
            insertCommand.CommandText =
                "INSERT INTO ip(ip, country, countryCode, continent, continentCode, latitude, longitude, locationAccuracyRadius, isp, abuseEmail) VALUES(@ip, @country, @countryCode, @continent, @continentCode, @latitude, @longitude, @locationAccuracyRadius, @isp, @abuseContactEmail)";
            insertCommand.Parameters.AddWithValue("@ip", report.SourceIpAddress);
            insertCommand.Parameters.AddWithValue("@country", ipAddressGeography.Geography.Country);
            insertCommand.Parameters.AddWithValue("@countryCode", ipAddressGeography.Geography.CountryCode);
            insertCommand.Parameters.AddWithValue("@continent", ipAddressGeography.Geography.Continent);
            insertCommand.Parameters.AddWithValue("@continentCode", ipAddressGeography.Geography.ContinentCode);
            insertCommand.Parameters.AddWithValue("@latitude", ipAddressGeography.Geography.Latitude);
            insertCommand.Parameters.AddWithValue("@longitude", ipAddressGeography.Geography.Longitude);
            insertCommand.Parameters.AddWithValue("@locationAccuracyRadius",
                ipAddressGeography.Geography.LocationAccuracyRadius);
            insertCommand.Parameters.AddWithValue("@isp", ipAddressGeography.Geography.ISP);
            if (!string.IsNullOrEmpty(abuseContactEmail))
                insertCommand.Parameters.AddWithValue("@abuseContactEmail", abuseContactEmail);
            else
                insertCommand.Parameters.AddWithValue("@abuseContactEmail", DBNull.Value);
            insertCommand.ExecuteNonQuery();
        }
    }
    else
    {
        rdr.Read();
        ipAddressGeography = new IpAddressGeography
        {
            Geography = new Geography
            {
                Country = rdr.GetString(2),
                CountryCode = rdr.GetString(3),
                Continent = rdr.GetString(4),
                ContinentCode = rdr.GetString(5),
                Latitude = rdr.GetDouble(6),
                Longitude = rdr.GetDouble(7),
                LocationAccuracyRadius = rdr.GetInt32(8),
                ISP = rdr.GetString(9)
            },
            AbuseEmail = rdr.IsDBNull(10) ? null : rdr.GetString(10)
        };
    }

    // EMAIL REPORTS
    if (config.EmailReport.Enabled)
    {
        if (ipAddressGeography != null)
        {
            if (string.IsNullOrEmpty(ipAddressGeography.AbuseEmail))
            {
                ipAddressGeography.AbuseEmail = await Ripe.QueryAbuseContact(report.SourceIpAddress);
            }

            if (string.IsNullOrEmpty(ipAddressGeography.AbuseEmail))
                Logger.Log(Logger.Level.Error, $"Unable to query abuse contact for {report.SourceIpAddress}");
            else
                EmailReport.GenerateReport(config, report, ipAddressGeography.AbuseEmail);
        }
        else
        {
            Logger.Log(Logger.Level.Error, $"No IP Geography for {report.SourceIpAddress}");
        }
    }
    // END EMAIL REPORTS

    var outputFolder = arguments.Value.Output;
    //outputFolder = Path.Combine(outputFolder, $"{city.Country.IsoCode}", $"{city.City.Name}");

    if (ipAddressGeography != null)
    {
        if (!string.IsNullOrEmpty(ipAddressGeography.Geography.ISP) && ipAddressGeography.Geography.ISP != "-")
            outputFolder = Path.Combine(outputFolder, "ISP", $"{ipAddressGeography.Geography.ISP}");
        else
            outputFolder = Path.Combine(outputFolder, "Countries", $"{ipAddressGeography.Geography.CountryCode}");
    }
    else
    {
        outputFolder = Path.Combine(outputFolder, "Unknown");
    }

    outputFolder = Path.Combine(outputFolder, report.DateTime.ToString("yyyy-MM-dd"));

    if (!Directory.Exists(outputFolder)) Directory.CreateDirectory(outputFolder);

    var outputFile = Path.Combine(outputFolder, $"{report.SourceIpAddress}.json");

    File.WriteAllText(outputFile, xarfReporter.GetReport(report));
    xarfReportFiles++;

    Logger.Log(Logger.Level.Debug,
        $"{report.DateTime:yyyy-MM-dd HH:mm:ss.ffff}| {report.SourceIpAddress}:{report.SourcePort}");
}

Logger.Log($"Created {xarfReportFiles} XARF report files!");

var allReports = reports.Select(x => xarfReporter.GetReport(x)).ToList();
if (allReports.Any())
    File.WriteAllLines(Path.Combine(config.OutputPath, "combined_reports.json"), allReports);