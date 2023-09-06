namespace RestSharpClient.Model.RestfulBooker;

using Newtonsoft.Json;

public class Auth
{
    [JsonProperty("token")]
    public string? Token { get; set; }
}
