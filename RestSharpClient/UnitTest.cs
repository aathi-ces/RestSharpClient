using System.Collections.Generic;
using System.Net;
using Helpers;
using RestSharp;
using RestSharpClient.ApiClient;
using RestSharpClient.Helpers.RestfulBooker;
using RestSharpClient.Model.RestfulBooker;
using Xunit;
using System.Net.Mime;
using Newtonsoft.Json;
using RestSharpClient.Helpers;
using System.Runtime.Caching;
using Microsoft.Extensions.Caching.Memory;
using MemoryCache = Microsoft.Extensions.Caching.Memory.MemoryCache;
using RestSharpClient.Utils;
using RestSharp.Serializers;


namespace Microsoft.CodeAnalysis.CSharp.UnitTests
{
    public class UnitTest
    {
        IApiService client = new ApiService(ConfigurationHelper.BuildConfiguration().RestfulBooker.BaseUrl);

        [Fact(DisplayName = "GET /api/users returns http status code 200")]
        [Trait("Domain", "ReqRes")]
        public void VerifyGetUsersApiReturns200()
        {
            var cache = new InMemoryCache();
            var errorLogger = new ErrorLogger();
            var serializer = new RestSharpClient.Utils.JsonSerializer();
            client = new ApiService(cache, serializer, errorLogger, ConfigurationHelper.BuildConfiguration().ReqRes.BaseUrl);

            ApiMethodParams apiMethodParams = new()
            {
                Uri = ConfigurationHelper.BuildConfiguration().ReqRes.Resources.ListUsers,
            };

            RestResponse<Users> response = client.GetFromCacheAsyncMethod<Users>(apiMethodParams, "UserList").Result;
            Users users = JsonConvert.DeserializeObject<Users>(response.Content!)!;

            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            Assert.Equal(MediaTypeNames.Application.Json, response.ContentType);
            Assert.NotEmpty(users.Data);
            Assert.All(users.Data, item => Assert.IsType<Datum>(item));
        }

        [Theory(DisplayName = "GET /api/booking returns http status code 200")]
        [Trait("Domain", "RestfulBooking")]
        [InlineData()]
        [InlineData("firstName", "lastName")]
        [InlineData("lastName")]
        public void VerifyGetAllBookingIdsApi(params string[] keys)
        {
            ApiMethodParams apiMethodParams = new()
            {
                Uri = ConfigurationHelper.BuildConfiguration().RestfulBooker.Resources.Booking,
                QueryParams = BookingHelper.GenerateBookingQueryParams(client, keys)
            };

            IApiService service = client.GetMethod(apiMethodParams);
            RestResponse response = service.GetResponse();
            List<BookingIds> bookings = service.GetValueInList<BookingIds>();

            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            Assert.Equal(MediaTypeNames.Application.Json, response.ContentType);
            Assert.NotEmpty(bookings);
            Assert.All(bookings, item => Assert.IsType<BookingIds>(item));
        }

        [Fact(DisplayName = "GET /api/booking/{id} returns http status code 200")]
        [Trait("Domain", "RestfulBooking")]
        public void VerifyGetBookingApi()
        {
            ApiMethodParams apiMethodParams = new()
            {
                Uri = ConfigurationHelper.BuildConfiguration().RestfulBooker.Resources.BookingId,
                UrlSegments = new Dictionary<string, string>() { { "id", BookingHelper.GetRandomBookingId(client).Bookingid.ToString()! } }
            };

            IApiService service = client.GetMethod(apiMethodParams);
            RestResponse response = service.GetResponse();
            Booking booking = service.GetValue<Booking>();

            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            Assert.Equal(MediaTypeNames.Application.Json, response.ContentType);
        }

        [Fact(DisplayName = "POST /api/booking returns http status code 200")]
        [Trait("Domain", "RestfulBooking")]
        public void VerifyPostBookingApi()
        {
            Booking expectedBooking = BookingHelper.BookingData;

            IApiService service = client.PostMethod(expectedBooking, ConfigurationHelper.BuildConfiguration().RestfulBooker.Resources.Booking);
            RestResponse response = service.GetResponse();
            Bookings createdBooking = service.GetValue<Bookings>();
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            Assert.Equal(MediaTypeNames.Application.Json, response.ContentType);
            Assert.Equivalent(expectedBooking, createdBooking.Booking);
        }

        [Fact(DisplayName = "PUT /api/booking/{id} returns http status code 200")]
        [Trait("Domain", "RestfulBooking")]
        public void VerifyPutBookingApi()
        {
            Booking bookingData = BookingHelper.BookingData;
            Bookings bookingRecord = client.PostMethod(bookingData, ConfigurationHelper.BuildConfiguration().RestfulBooker.Resources.Booking).GetValue<Bookings>();

            Booking updatedBookingData = bookingRecord.Booking;
            updatedBookingData.With(p =>
            {
                p.Firstname = $"{bookingData.Firstname}_updated";
                p.Lastname = $"{bookingData.Lastname}_updated";
                p.Totalprice = bookingData.Totalprice + 100;
            });

            ApiMethodParams apiMethodParams = new()
            {
                Uri = ConfigurationHelper.BuildConfiguration().RestfulBooker.Resources.BookingId,
                UrlSegments = new Dictionary<string, string>() { { "id", bookingRecord.Bookingid.ToString() } },
                Payload = updatedBookingData
            };

            IApiService service = client.PutMethod(apiMethodParams);
            RestResponse response = service.GetResponse();
            Booking createdBooking = service.GetValue<Booking>();
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            Assert.Equal(MediaTypeNames.Application.Json, response.ContentType);
            Assert.Equivalent(updatedBookingData, createdBooking);
        }

        [Fact(DisplayName = "PATCH /api/booking/{id} returns http status code 200")]
        [Trait("Domain", "RestfulBooking")]
        public void VerifyPatchBookingApi()
        {
            Booking bookingData = BookingHelper.BookingData;
            Bookings bookingRecord = client.PostMethod(bookingData, ConfigurationHelper.BuildConfiguration().RestfulBooker.Resources.Booking).GetValue<Bookings>();

            var updatedBookingData = JsonConvert.SerializeObject(
               new
                {
                   firstname = $"{bookingData.Firstname}_updated",
                   lastname = $"{bookingData.Lastname}_updated",
                   totalprice = bookingData.Totalprice + 100
               });

            ApiMethodParams apiMethodParams = new()
            {
                Uri = ConfigurationHelper.BuildConfiguration().RestfulBooker.Resources.BookingId,
                UrlSegments = new Dictionary<string, string>() { { "id", bookingRecord.Bookingid.ToString() } },
                Payload = updatedBookingData
            };

            IApiService service = client.PatchMethod(apiMethodParams);
            RestResponse response = service.GetResponse();
            Booking createdBooking = service.GetValue<Booking>();
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            Assert.Equal(MediaTypeNames.Application.Json, response.ContentType);
            Assert.Equivalent(new Booking
            {
                Firstname = createdBooking.Firstname,
                Lastname = createdBooking.Lastname,
                Totalprice = createdBooking.Totalprice,
                Depositpaid = bookingData.Depositpaid,
                Bookingdates = bookingData.Bookingdates,
                Additionalneeds = bookingData.Additionalneeds
            }, createdBooking);
        }

        [Fact(DisplayName = "DELETE /api/booking/{id} returns http status code 201")]
        [Trait("Domain", "RestfulBooking")]
        public void VerifyDeleteBookingApi()
        {
            Booking bookingData = BookingHelper.BookingData;
            Bookings bookingRecord = client.PostMethod(bookingData, ConfigurationHelper.BuildConfiguration().RestfulBooker.Resources.Booking).GetValue<Bookings>();

            ApiMethodParams deleteApiMethodParams = new()
            {
                Uri = ConfigurationHelper.BuildConfiguration().RestfulBooker.Resources.BookingId,
                UrlSegments = new Dictionary<string, string>() { { "id", bookingRecord.Bookingid.ToString() } },
            };

            IApiService service = client.DeleteMethod(deleteApiMethodParams);
            RestResponse response = service.GetResponse();
            Assert.Equal(HttpStatusCode.Created, response.StatusCode);
            Assert.Equal(MediaTypeNames.Text.Plain, response.ContentType);

            ApiMethodParams getApiMethodParams = new()
            {
                Uri = ConfigurationHelper.BuildConfiguration().RestfulBooker.Resources.BookingId,
                UrlSegments = new Dictionary<string, string>() { { "id", bookingRecord.Bookingid.ToString()! } }
            };

            IApiService getService = client.GetMethod(getApiMethodParams);
            RestResponse getResponse = getService.GetResponse();
            Assert.Equal("Not Found", getResponse.Content);
        }
    }
}
