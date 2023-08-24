using Newtonsoft.Json;
using RestSharp;
using Newtonsoft.Json.Converters;
using System.Globalization;
using System.Reflection.PortableExecutable;
using System.Net.Mime;
using RestSharp.Authenticators;
using RestSharpClient.Model.RestfulBooker;
using Settings;
using RestSharpClient.Utils;
using RestSharp.Serializers;

namespace RestSharpClient.ApiClient
{
    public partial class ApiService : BaseClient, IApiService, IDisposable
    {
        readonly RestClient restClient;

        public RestResponse? Response { get; set; }

        public ApiService(ICacheService cache,
                          IDeserializer serializer,
                          IErrorLogger errorLogger,
                          Uri baseUrl) : base(cache, serializer, errorLogger, baseUrl)
        {

            RestClientOptions restClientOptions = new()
            {
                BaseUrl = baseUrl,
                Authenticator = new HttpBasicAuthenticator("admin", "password123")
            };
            restClient = new (restClientOptions);
        }

        public ApiService(Uri baseUrl)
        {
            RestClientOptions restClientOptions = new()
            {
                BaseUrl = baseUrl,
                Authenticator = new HttpBasicAuthenticator("admin", "password123")
            };
            restClient = new(restClientOptions);
        }

        public IApiService PostMethod(object requestObject, string uri, Dictionary<string, string>? headers = null)
        {

            var request = new RestRequest() { Resource = uri, Method = Method.Post, RequestFormat = DataFormat.Json };
            request.AddHeader("Accept", "application/json");
            request.AddHeader("Content-Type", "application/json");
            Response = Execute(restClient, request, requestObject, headers!);
            return this;
        }

        public IApiService GetMethod(string uri, Dictionary<string, string>? headers = null)
        {
            var request = new RestRequest() { Resource = uri, Method = Method.Get, RequestFormat = DataFormat.Json };
            Response = Execute(restClient, request, headers);
            return this;
        }

        public static RestRequest SetRequestElements(RestRequest request, ApiMethodParams apiMethodParams) {
            request.AddHeader("Accept", MediaTypeNames.Application.Json);
            request.AddHeader("Content-Type", MediaTypeNames.Application.Json);
            apiMethodParams.Headers?.Where(l => l.Key != null).ToList().ForEach(header => request.AddHeader(header.Key, header.Value));
            apiMethodParams.UrlSegments?.Where(l => l.Key != null).ToList().ForEach(segment => request.AddUrlSegment(segment.Key, segment.Value));
            apiMethodParams.QueryParams?.Where(l => l.Key != null).ToList().ForEach(param => request.AddQueryParameter(param.Key, param.Value));
            return request;
        }

        public IApiService GetMethod(ApiMethodParams apiMethodParams)
        {
            var request = new RestRequest() { Resource = apiMethodParams.Uri!, Method = Method.Get, RequestFormat = DataFormat.Json };
            SetRequestElements(request, apiMethodParams);
            Response = Execute(restClient, request, apiMethodParams.Headers);
            return this;
        }

        public T GetFromBase<T>(ApiMethodParams apiMethodParams)
        {
            var request = new RestRequest() { Resource = apiMethodParams.Uri!, Method = Method.Get, RequestFormat = DataFormat.Json };
            SetRequestElements(request, apiMethodParams);
            return Get<T>(request);
        }

        public IApiService PutMethod(object requestObject, string uri, Dictionary<string, string>? headers = null)
        {
            var request = new RestRequest() { Resource = uri, Method = Method.Put, RequestFormat = DataFormat.Json };
            Response = Execute(restClient, request, requestObject, headers!);
            return this;
        }

        public IApiService PutMethod(ApiMethodParams apiMethodParams)
        {
            var request = new RestRequest() { Resource = apiMethodParams.Uri!, Method = Method.Put, RequestFormat = DataFormat.Json };
            request = SetRequestElements(request, apiMethodParams);
            Response = Execute(restClient, request, apiMethodParams.Payload, apiMethodParams.Headers!);
            return this;
        }

        public IApiService PatchMethod(object requestObject, string uri, Dictionary<string, string>? headers = null)
        {
            var request = new RestRequest() { Resource = uri, Method = Method.Patch, RequestFormat = DataFormat.Json };
            Response = Execute(restClient, request, requestObject, headers!);
            return this;
        }

        public IApiService PatchMethod(ApiMethodParams apiMethodParams)
        {
            var request = new RestRequest() { Resource = apiMethodParams.Uri!, Method = Method.Patch, RequestFormat = DataFormat.Json };
            request = SetRequestElements(request, apiMethodParams);
            Response = Execute(restClient, request, apiMethodParams.Payload, apiMethodParams.Headers!);
            return this;
        }

        public IApiService DeleteMethod(string uri, Dictionary<string, string>? headers = null)
        {
            var request = new RestRequest() { Resource = uri, Method = Method.Delete, RequestFormat = DataFormat.Json };
            Response = Execute(restClient, request, null, headers!);
            return this;
        }

        public IApiService DeleteMethod(ApiMethodParams apiMethodParams)
        {
            var request = new RestRequest() { Resource = apiMethodParams.Uri!, Method = Method.Delete, RequestFormat = DataFormat.Json };
            request = SetRequestElements(request, apiMethodParams);
            Response = Execute(restClient, request, apiMethodParams.Payload, apiMethodParams.Headers!);
            return this;
        }

        private static RestResponse Execute(RestClient client, RestRequest request, object? requestObject = null, Dictionary<string, string>? headers = null)
        {
            if (headers != null)
            {
                foreach (var header in headers)
                    request.AddHeader(header.Key, header.Value);
            }

            if (requestObject != null)
                request.AddJsonBody(requestObject);

            return client.Execute(request);
        }

        public T GetValue<T>()
        {
            return JsonConvert.DeserializeObject<T>(Response!.Content!, Converter.Settings)!;
        }

        public List<T> GetValueInList<T>()
        {
            return JsonConvert.DeserializeObject<List<T>>(Response!.Content!)!;
        }

        public RestResponse GetResponse()
        {
            return Response!;
        }

        public void Dispose()
        {
            throw new NotImplementedException();
        }
    }

    public partial class ApiService<T>
    {
        public static T FromJson(string json) => JsonConvert.DeserializeObject<T>(json, Converter.Settings)!;
    }

    public static class Serialize
    {
        public static string ToJson(this Auth self) => JsonConvert.SerializeObject(self, Converter.Settings);
    }

    internal static class Converter
    {
        public static readonly JsonSerializerSettings Settings = new JsonSerializerSettings
        {
            NullValueHandling = NullValueHandling.Ignore,
            MissingMemberHandling = MissingMemberHandling.Ignore,
            MetadataPropertyHandling = MetadataPropertyHandling.Ignore,
            DateParseHandling = DateParseHandling.None,
            Converters =
            {
                new IsoDateTimeConverter { DateTimeStyles = DateTimeStyles.AssumeUniversal }
            },
        };
    }
}
