namespace Settings;

using System;
using System.Globalization;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

public class AppSettings
{
    [JsonProperty(nameof(ReqRes))]
    public ReqRes? ReqRes { get; set; }

    [JsonProperty("Restful-Booker")]
    public RestfulBooker? RestfulBooker { get; set; }
}

public class ReqRes
{
    [JsonProperty(nameof(BaseUrl))]
    public Uri? BaseUrl { get; set; }

    [JsonProperty(nameof(Resources))]
    public ReqResResources? Resources { get; set; }
}

public class ReqResResources
{
    [JsonProperty(nameof(ListUsers))]
    public string? ListUsers { get; set; }
}

public class RestfulBooker
{
    [JsonProperty(nameof(BaseUrl))]
    public Uri? BaseUrl { get; set; }

    [JsonProperty(nameof(Resources))]
    public RestfulBookerResources? Resources { get; set; }
}

public class RestfulBookerResources
{
    [JsonProperty(nameof(Auth))]
    public string? Auth { get; set; }

    [JsonProperty(nameof(Booking))]
    public string? Booking { get; set; }

    [JsonProperty(nameof(BookingId))]
    public string? BookingId { get; set; }
}

public static class Serialize
{
    public static string ToJson(this AppSettings self) => JsonConvert.SerializeObject(self, Settings.Converter.Settings);
}

internal static class Converter
{
    public static readonly JsonSerializerSettings Settings = new JsonSerializerSettings
    {
        MetadataPropertyHandling = MetadataPropertyHandling.Ignore,
        DateParseHandling = DateParseHandling.None,
        Converters =
            {
                new IsoDateTimeConverter { DateTimeStyles = DateTimeStyles.AssumeUniversal }
            },
    };
}
