using System.Data.SQLite;
using System.Reflection;
using ITHock.XarfReportGenerator.Plugin.Utils;

namespace ITHock.XarfReportGenerator;

public class CacheDatabase
{
    private SQLiteConnection? _connection;

    public CacheDatabase()
    {
    }

    public void Initialize()
    {
        _connection =
            new SQLiteConnection(
                $"Data Source={Path.GetDirectoryName(Assembly.GetEntryAssembly()?.Location)}\\cache.db;Version=3;");
        _connection.Open();

        {
            var cmd = new SQLiteCommand(_connection);
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
    }

    public async Task<IpAddressGeography?> GetCachedIp(string ip)
    {
        await using var cmd = new SQLiteCommand("SELECT * FROM ip WHERE ip=@ip", _connection);
        cmd.Parameters.AddWithValue("@ip", ip);
        await using var rdr = cmd.ExecuteReader();

        if (!rdr.HasRows)
            return null;

        rdr.Read();
        var ipAddressGeography = new IpAddressGeography
        {
            Geography = new IpAddressGeography.IpGeography
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

        return ipAddressGeography;
    }

    public async Task<IpAddressGeography?> AddCachedIp(string ip)
    {
        var ipAddressGeography = await IpLookup.IpBan_LookupIp(ip);
        var abuseContactEmail = await Ripe.QueryAbuseContact(ip);
        if (ipAddressGeography != null && !ipAddressGeography.Error)
        {
            await using var insertCommand = new SQLiteCommand(_connection);
            insertCommand.CommandText =
                "INSERT INTO ip(ip, country, countryCode, continent, continentCode, latitude, longitude, locationAccuracyRadius, isp, abuseEmail) VALUES(@ip, @country, @countryCode, @continent, @continentCode, @latitude, @longitude, @locationAccuracyRadius, @isp, @abuseContactEmail)";
            insertCommand.Parameters.AddWithValue("@ip", ip);
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

            return ipAddressGeography;
        }

        return null;
    }
}