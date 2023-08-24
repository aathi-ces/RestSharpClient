using System;
namespace RestSharpClient.Utils
{
    using Newtonsoft.Json;
    using RestSharp;
    using RestSharp.Serializers;
    using System.IO;

    public class JsonSerializer : IRestSerializer
    {
        public static string Serialize(object obj) => JsonSerializer.Serialize(obj);

        public string Serialize(Parameter bodyParameter) => Serialize(bodyParameter.Value!);

        public static T Deserialize<T>(RestResponse response) => JsonSerializer.Deserialize<T>(response);

        public string[] SupportedContentTypes { get; } = {
        "application/json", "text/json", "text/x-json", "text/javascript", "*+json"
    };

        public string ContentType { get; set; } = "application/json";

        public DataFormat DataFormat { get; } = DataFormat.Json;

        public ISerializer Serializer => throw new NotImplementedException();

        public IDeserializer Deserializer => throw new NotImplementedException();

        public string[] AcceptedContentTypes => throw new NotImplementedException();

        public SupportsContentType SupportsContentType => throw new NotImplementedException();
    }
}

