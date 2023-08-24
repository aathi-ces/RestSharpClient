using System;
using RestSharp.Serializers;
using RestSharpClient.Utils;
using RestSharp;

namespace RestSharpClient.ApiClient
{
    using RestSharp;
    using System;
    using System.IO;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;

    public class BaseClient : JsonSerializer, RestSharp.IRestClient
    {
        readonly Uri BaseUrl;
        protected ICacheService _cache;
        protected IErrorLogger _errorLogger;

        public ReadOnlyRestClientOptions Options => throw new NotImplementedException();

        public RestSerializers Serializers => throw new NotImplementedException();

        public DefaultParameters DefaultParameters => throw new NotImplementedException();

        public BaseClient(ICacheService cache,
                          IDeserializer serializer,
                          IErrorLogger errorLogger,
                          Uri baseUrl)
        {
            _cache = cache;
            _errorLogger = errorLogger;
            //AddHandler("application/json", serializer);
            //AddHandler("text/json", serializer);
            //AddHandler("text/x-json", serializer);
            BaseUrl = baseUrl;
        }

        public BaseClient() { }

        private void TimeoutCheck(RestRequest request, RestResponse response)
        {
            if (response.StatusCode == 0)
            {
                LogError(BaseUrl, request, response);
            }
        }

        //public T Get<T>(RestRequest request) where T : new()
        public T Get<T>(RestRequest request)
        {
            var response = ExecuteAsync<T>(request);
            if (response.Result.StatusCode == System.Net.HttpStatusCode.OK)
            {
                return Deserialize<T>(response.Result);
            }
            else
            {
                LogError(BaseUrl, request, response.Result);
                return default(T);
            }
        }

        public T GetFromCache<T>(RestRequest request, string cacheKey)
          where T : class, new()
        {
            var item = _cache.Get<T>(cacheKey);
            if (item == null)
            {
                var response = ExecuteAsync<T>(request);
                if (response.Result.StatusCode == System.Net.HttpStatusCode.OK)
                {
                    _cache.Set(cacheKey, response.Result.Content!, 1);
                    item = Deserialize<T>(response.Result);
                }
                else
                {
                    LogError(BaseUrl, request, response.Result);
                    return default(T)!;
                }
            }
            return item!;
        }

        private void LogError(Uri BaseUrl,
                              RestRequest request,
                              RestResponse response)
        {
            //Get the values of the parameters passed to the API
            string parameters = string.Join(", ", request.Parameters.Select(x => x.Name!.ToString() + "=" + ((x.Value == null) ? "NULL" : x.Value)).ToArray());

            //Set up the information message with the URL, 
            //the status code, and the parameters.
            string info = "Request to " + BaseUrl.AbsoluteUri
                          + request.Resource + " failed with status code "
                          + response.StatusCode + ", parameters: "
                          + parameters + ", and content: " + response.Content;

            //Acquire the actual exception
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

            //Log the exception and info message
            _errorLogger.LogError(ex, info);
        }

        public Task<RestResponse> ExecuteAsync(RestRequest request, CancellationToken cancellationToken = default)
        {
            var response = ExecuteAsync(request);
            TimeoutCheck(request, response.Result);
            return response;
        }

        public Task<RestResponse> ExecuteAsync<T>(RestRequest request, CancellationToken cancellationToken = default)
        {
            var response = ExecuteAsync<T>(request);
            TimeoutCheck(request, response.Result);
            return response;
        }

        public Task<Stream?> DownloadStreamAsync(RestRequest request, CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }

        public void Dispose()
        {
            throw new NotImplementedException();
        }
    }
}

