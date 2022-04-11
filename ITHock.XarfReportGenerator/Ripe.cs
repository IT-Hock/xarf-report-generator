using System.Net.Http.Headers;
using Newtonsoft.Json;

namespace ITHock.XarfReportGenerator;

public class Ripe
{
    [JsonObject]
    public class Link
    {
        [JsonProperty("type")]
        public string Type { get; set; } = "";

        [JsonProperty("href")]
        public string Href { get; set; } = "";
    }

    [JsonObject]
    public class PrimaryKey
    {
        [JsonProperty("value")]
        public string Value { get; set; } = "";
    }

    [JsonObject]
    public class Parameters
    {
        [JsonProperty("primary-key")]
        public PrimaryKey PrimaryKey { get; set; } = new();
    }

    [JsonObject]
    public class AbuseContacts
    {
        [JsonProperty("key")]
        public string Key { get; set; } = "";

        [JsonProperty("email")]
        public string Email { get; set; } = "";

        [JsonProperty("suspect")]
        public bool Suspect { get; set; }

        [JsonProperty("org-id")]
        public string OrgId { get; set; } = "";
    }

    [JsonObject]
    public class TermsAndConditions
    {
        [JsonProperty("type")]
        public string Type { get; set; } = "";

        [JsonProperty("href")]
        public string Href { get; set; } = "";
    }

    [JsonObject]
    public class AbuseContact
    {
        [JsonProperty("service")]
        public string Service { get; set; } = "";

        [JsonProperty("link")]
        public Link Link { get; set; } = new();

        [JsonProperty("parameters")]
        public Parameters Parameters { get; set; } = new();

        [JsonProperty("abuse-contacts")]
        public AbuseContacts AbuseContacts { get; set; } = new();

        [JsonProperty("terms-and-conditions")]
        public TermsAndConditions TermsAndConditions { get; set; } = new();
    }

    /// <summary>
    /// Returns the abuse contact email address for the given IP address.
    /// </summary>
    /// <param name="ipAddress">The IP address to lookup.</param>
    /// <returns>The abuse contact email address for the given IP address.</returns>
    public static async Task<string?> QueryAbuseContact(string ipAddress)
    {
        using var client = new HttpClient();

        client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
        var response = await client.GetAsync($"https://rest.db.ripe.net/abuse-contact/{ipAddress}");
        if (!response.IsSuccessStatusCode) return null;

        var json = await response.Content.ReadAsStringAsync();
        var abuseContact = JsonConvert.DeserializeObject<AbuseContact>(json);
        return abuseContact?.AbuseContacts.Email;
    }
}