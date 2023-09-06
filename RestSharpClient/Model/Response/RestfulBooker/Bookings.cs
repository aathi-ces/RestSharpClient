namespace RestSharpClient.Model.RestfulBooker;

using System;
using Newtonsoft.Json;

public class Bookings
{
    [JsonProperty("bookingid")]
    public long Bookingid { get; set; }
    [JsonProperty("booking")]
    public Booking? Booking { get; set; }
}

public class Booking
{
    [JsonProperty("firstname")]
    public string? Firstname { get; set; }

    [JsonProperty("lastname")]
    public string? Lastname { get; set; }

    [JsonProperty("totalprice")]
    public long Totalprice { get; set; }

    [JsonProperty("depositpaid")]
    public bool Depositpaid { get; set; }

    [JsonProperty("bookingdates", NullValueHandling = NullValueHandling.Ignore)]
    public Bookingdates? Bookingdates { get; set; }

    [JsonProperty("additionalneeds")]
    public string? Additionalneeds { get; set; }
}

public class Bookingdates
{
    [JsonProperty("checkin", NullValueHandling = NullValueHandling.Ignore)]
    public DateTimeOffset? Checkin { get; set; }

    [JsonProperty("checkout", NullValueHandling = NullValueHandling.Ignore)]
    public DateTimeOffset? Checkout { get; set; }
}
