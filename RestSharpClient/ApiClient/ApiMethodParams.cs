using System;
namespace RestSharpClient.ApiClient
{
	public class ApiMethodParams
	{
        public string? Uri { get; set; }
		public Dictionary<string, string>? QueryParams { get; set; }
        public Dictionary<string, string>? Headers { get; set; }
        public Dictionary<string, string>? UrlSegments { get; set; }
        public Object? Payload { get; set; }
    }
}

