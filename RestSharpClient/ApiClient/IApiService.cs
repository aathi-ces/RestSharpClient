using RestSharp;

namespace RestSharpClient.ApiClient;
public interface IApiService
{
    IApiService PostMethod(object requestObject, string uri, Dictionary<string, string>? headers = null);

    IApiService GetMethod(string uri, Dictionary<string, string>? headers = null);

    IApiService GetMethod(ApiMethodParams apiMethodParams);

    T GetFromCacheMethod<T>(ApiMethodParams apiMethodParams, string cacheKey) where T : class, new();

    Task<RestResponse<T>> GetFromCacheAsyncMethod<T>(ApiMethodParams apiMethodParams, string cacheKey) where T : class, new();

    IApiService PutMethod(object requestObject, string uri, Dictionary<string, string>? headers = null);

    IApiService PutMethod<T>(ApiMethodParams<T> apiMethodParams);
    IApiService PatchMethod(object requestObject, string uri, Dictionary<string, string>? headers = null);

    IApiService PatchMethod<T>(ApiMethodParams<T> apiMethodParams);

    IApiService DeleteMethod(string uri, Dictionary<string, string>? headers = null);

    IApiService DeleteMethod<T>(ApiMethodParams<T> apiMethodParams);

    RestResponse GetResponse();

    T GetValue<T>();

    List<T> GetValueInList<T>();
}
