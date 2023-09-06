using Helpers;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using RestSharp;
using RestSharp.Authenticators;
using RestSharpClient.Model.RestfulBooker;
using RestSharpClient.Utils;
using System.Globalization;
using System.Net;
using System.Net.Mime;
using JsonSerializer = RestSharpClient.Utils.JsonSerializer;

namespace RestSharpClient.ApiClient;
public class ApiService : BaseClient, IApiService
{
    private RestResponse? Response;

    public ApiService(ICacheService cache,
                      JsonSerializer serializer,
                      IErrorLogger errorLogger,
                      Uri baseUrl) : base(cache, serializer, errorLogger, baseUrl)
    {
        RestClientOptions restClientOptions = new()
        {
            BaseUrl = baseUrl,
            Authenticator = new HttpBasicAuthenticator("admin", "password123")
        };
        restClient = new(restClientOptions);
    }

    public ApiService(Uri baseUrl) : base(baseUrl)
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
        request.AddHeader(nameof(HttpRequestHeader.Accept), MediaTypeNames.Application.Json);
        request.AddHeader(nameof(HttpRequestHeader.ContentType), MediaTypeNames.Application.Json);
        Response = Execute(restClient, request, requestObject, headers!);
        return this;
    }

    public IApiService GetMethod(string uri, Dictionary<string, string>? headers = null)
    {
        var request = new RestRequest() { Resource = uri, Method = Method.Get, RequestFormat = DataFormat.Json };
        Response = Execute(restClient, request, headers);
        return this;
    }

    public static RestRequest SetRequestElements(RestRequest request, ApiMethodParams apiMethodParams)
    {
        request.AddHeader(nameof(HttpRequestHeader.Accept), MediaTypeNames.Application.Json);
        request.AddHeader(nameof(HttpRequestHeader.ContentType), MediaTypeNames.Application.Json);
        apiMethodParams.Headers?.Where(l => l.Key != null).ToList().ForEach(header => request.AddHeader(header.Key, header.Value));
        apiMethodParams.UrlSegments?.Where(l => l.Key != null).ToList().ForEach(segment => request.AddUrlSegment(segment.Key, segment.Value));
        apiMethodParams.QueryParams?.Where(l => l.Key != null).ToList().ForEach(param => request.AddQueryParameter(param.Key, param.Value));
        return request;
    }

    public static RestRequest SetRequestElements<T>(RestRequest request, ApiMethodParams<T> apiMethodParams)
    {
        request.AddHeader(nameof(HttpRequestHeader.Accept), MediaTypeNames.Application.Json);
        request.AddHeader(nameof(HttpRequestHeader.ContentType), MediaTypeNames.Application.Json);
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

    public T GetFromCacheMethod<T>(ApiMethodParams apiMethodParams, string cacheKey) where T : class, new()
    {
        var request = new RestRequest() { Resource = apiMethodParams.Uri!, Method = Method.Get, RequestFormat = DataFormat.Json };
        SetRequestElements(request, apiMethodParams);
        return GetFromCache<T>(request, cacheKey);
    }

    public Task<RestResponse<T>> GetFromCacheAsyncMethod<T>(ApiMethodParams apiMethodParams, string cacheKey) where T : class, new()
    {
        var request = new RestRequest() { Resource = apiMethodParams.Uri!, Method = Method.Get, RequestFormat = DataFormat.Json };
        SetRequestElements(request, apiMethodParams);
        return GetFromCacheAsync<T>(request, cacheKey);
    }

    public IApiService PutMethod(object requestObject, string uri, Dictionary<string, string>? headers = null)
    {
        var request = new RestRequest() { Resource = uri, Method = Method.Put, RequestFormat = DataFormat.Json };
        Response = Execute(restClient, request, requestObject, headers!);
        return this;
    }

    public IApiService PutMethod<T>(ApiMethodParams<T> apiMethodParams)
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

    public IApiService PatchMethod<T>(ApiMethodParams<T> apiMethodParams)
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

    public IApiService DeleteMethod<T>(ApiMethodParams<T> apiMethodParams)
    {
        var request = new RestRequest() { Resource = apiMethodParams.Uri!, Method = Method.Delete, RequestFormat = DataFormat.Json };
        request = SetRequestElements(request, apiMethodParams);
        Response = Execute(restClient, request, apiMethodParams.Payload, apiMethodParams.Headers!);
        return this;
    }

    private static RestResponse Execute(RestClient client, RestRequest request, object? requestObject = null, Dictionary<string, string>? headers = null)
    {
        RestResponse? response = null;
        if (headers != null)
        {
            foreach (var header in headers)
                request.AddHeader(header.Key, header.Value);
        }

        if (requestObject != null)
            request.AddJsonBody(requestObject);

        try
        {
            response = client.Execute(request);
        }
        catch (Exception exception)
        {
            new ErrorLogger().LogError(exception, "");
        }

        return response!;
    }

    private static T Execute<T>(RestClient client, RestRequest request, object? requestObject = null, Dictionary<string, string>? headers = null)
    {
        RestResponse? response = null;
        if (headers != null)
        {
            foreach (var header in headers)
                request.AddHeader(header.Key, header.Value);
        }

        if (requestObject != null)
            request.AddJsonBody(requestObject);

        try
        {
            response = client.Execute(request);
        }
        catch (Exception exception)
        {
            new ErrorLogger().LogError(exception, "");
        }

        return new ApiService(ConfigurationHelper.BuildConfiguration().RestfulBooker.BaseUrl).GetValue<T>();
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
    public static readonly JsonSerializerSettings Settings = new()
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
