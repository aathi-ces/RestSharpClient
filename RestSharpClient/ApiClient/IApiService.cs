using System.Collections.Generic;
using RestSharp;

namespace RestSharpClient.ApiClient
{
    public interface IApiService
    {
        IApiService PostMethod(object requestObject, string uri, Dictionary<string, string>? headers = null);

        IApiService GetMethod(string uri, Dictionary<string, string>? headers = null);

        IApiService GetMethod(ApiMethodParams apiMethodParams);

        IApiService PutMethod(object requestObject, string uri, Dictionary<string, string>? headers = null);

        IApiService PutMethod(ApiMethodParams apiMethodParams);

        IApiService PatchMethod(object requestObject, string uri, Dictionary<string, string>? headers = null);

        IApiService PatchMethod(ApiMethodParams apiMethodParams);

        IApiService DeleteMethod(string uri, Dictionary<string, string>? headers = null);

        IApiService DeleteMethod(ApiMethodParams apiMethodParams);

        RestResponse GetResponse();

        T GetValue<T>();

        List<T> GetValueInList<T>();
    }
}
