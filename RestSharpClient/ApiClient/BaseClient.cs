using System;
using RestSharp.Serializers;
using RestSharpClient.Utils;
using RestSharp;

namespace RestSharpClient.ApiClient
{
    using RestSharp;
    using Settings;
    using System;
    using System.IO;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;

    public class BaseClient : JsonSerializer
    {
        readonly Uri BaseUrl;
        protected RestClient restClient;
        protected ICacheService _cache;
        protected IErrorLogger _errorLogger;
        protected JsonSerializer _serializer;

        public ReadOnlyRestClientOptions Options => throw new NotImplementedException();

        public RestSerializers Serializers => throw new NotImplementedException();

        public DefaultParameters DefaultParameters => throw new NotImplementedException();

        public BaseClient(ICacheService cache,
                          JsonSerializer serializer,
                          IErrorLogger errorLogger,
                          Uri baseUrl)
        {
            _cache = cache;
            _errorLogger = errorLogger;
            _serializer = serializer;
            restClient = new RestClient(baseUrl);
            BaseUrl = baseUrl;
        }

        public BaseClient(Uri baseUrl) => BaseUrl = baseUrl;

        public void TimeoutCheck(RestRequest request, RestResponse response)
        {
            if (response.StatusCode == 0)
            {
                LogError(BaseUrl, request, response);
            }
        }

        public T Get<T>(RestRequest request)
        {
            var response = restClient.ExecuteAsync<T>(request);
            if (response.Result.StatusCode == System.Net.HttpStatusCode.OK)
            {
                return Newtonsoft.Json.JsonConvert.DeserializeObject<T>(response.Result.Content!)!;
            }
            else
            {
                LogError(BaseUrl, request, response.Result);
                return default(T)!;
            }
        }

        public T GetFromCache<T>(RestRequest request, string cacheKey)
          where T : class, new()
        {
            var item = _cache.Get<T>(cacheKey);
            if (item == null)
            {
                var response = restClient.ExecuteAsync<T>(request);
                if (response.Result.StatusCode == System.Net.HttpStatusCode.OK)
                {
                    _cache.Set(cacheKey, response.Result.Content!, 1);
                    item = Newtonsoft.Json.JsonConvert.DeserializeObject<T>(response.Result.Content!);
                }
                else
                {
                    LogError(BaseUrl, request, response.Result);
                    return default(T)!;
                }
            }
            return item!;
        }

        public Task<RestResponse<T>> GetFromCacheAsync<T>(RestRequest request, string cacheKey)
  where T : class, new()
        {
            Task<RestResponse<T>> response = null;
            var item = _cache.Get<T>(cacheKey);
            if (item == null)
            {
                response = restClient.ExecuteAsync<T>(request);
                if (response.Result.StatusCode == System.Net.HttpStatusCode.OK)
                {
                    _cache.Set(cacheKey, response.Result.Content!, 1);
                }
                else
                {
                    LogError(BaseUrl, request, response.Result);
                    return default(Task<RestResponse<T>>)!;
                }
            }
            return response!;
        }

        private void LogError(Uri BaseUrl,
                              RestRequest request,
                              RestResponse response)
        {
            string parameters = string.Join(", ", request.Parameters.Select(x => x.Name!.ToString() + "=" + ((x.Value == null) ? "NULL" : x.Value)).ToArray());

            string info = "Request to " + BaseUrl.AbsoluteUri
                          + request.Resource + " failed with status code "
                          + response.StatusCode + ", parameters: "
                          + parameters + ", and content: " + response.Content;

            Exception ex;
            if (response != null && response.ErrorException != null)
            {
                ex = response.ErrorException;
            }
            else
            {
                ex = new Exception(info);
                info = string.Empty;
            }

            _errorLogger.LogError(ex, info);
        }

        public void Dispose()
        {
            throw new NotImplementedException();
        }
    }
}

