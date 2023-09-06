namespace RestSharpClient.Model.RestfulBooker;

using Newtonsoft.Json;

public class BookingIds
{
    [JsonProperty("bookingid")]
    public long? Bookingid { get; set; }
}
