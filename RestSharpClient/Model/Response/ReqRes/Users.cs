namespace RestSharp;

using System;
using System.Collections.Generic;

using System.Globalization;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

public partial class Users
{
    [JsonProperty("page")]
    public long Page { get; set; }

    [JsonProperty("per_page")]
    public long PerPage { get; set; }

    [JsonProperty("total")]
    public long Total { get; set; }

    [JsonProperty("total_pages")]
    public long TotalPages { get; set; }

    [JsonProperty("data")]
    public List<Datum>? Data { get; set; }

    [JsonProperty("support")]
    public Support? Support { get; set; }
}

public partial class Datum
{
    [JsonProperty("id")]
    public long Id { get; set; }

    [JsonProperty("email")]
    public string? Email { get; set; }

    [JsonProperty("first_name")]
    public string? FirstName { get; set; }

    [JsonProperty("last_name")]
    public string? LastName { get; set; }

    [JsonProperty("avatar")]
    public Uri? Avatar { get; set; }
}

public partial class Support
{
    [JsonProperty("url")]
    public Uri? Url { get; set; }

    [JsonProperty("text")]
    public string? Text { get; set; }
}

public partial class Users
{
    public static Users FromJson(string json) => JsonConvert.DeserializeObject<Users>(json, Converter.Settings)!;
}

public static class Serialize
{
    public static string ToJson(this Users self) => JsonConvert.SerializeObject(self, Converter.Settings);
}

internal static class Converter
{
    public static readonly JsonSerializerSettings Settings = new()
    {
        MetadataPropertyHandling = MetadataPropertyHandling.Ignore,
        DateParseHandling = DateParseHandling.None,
        Converters =
            {
                new IsoDateTimeConverter { DateTimeStyles = DateTimeStyles.AssumeUniversal }
            },
    };
}
