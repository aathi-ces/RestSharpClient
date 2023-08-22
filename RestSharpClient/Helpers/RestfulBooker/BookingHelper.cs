using System;
using Helpers;
using RestSharpClient.ApiClient;
using RestSharpClient.Model.RestfulBooker;

namespace RestSharpClient.Helpers.RestfulBooker
{
	public static class BookingHelper
	{
        public static Booking BookingData { get; set; } = new Booking
            {
                Firstname = Faker.Name.First(),
                Lastname = Faker.Name.Last(),
                Totalprice = Faker.RandomNumber.Next(100, 1000),
                Depositpaid = Faker.Boolean.Random(),
                Bookingdates = new Bookingdates
                {
                    Checkin = DateTime.Parse("2023-01-02").ToUniversalTime().Date,
                    Checkout = DateTime.Parse("2023-01-02").ToUniversalTime().Date
                },
                Additionalneeds = "Breakfast"
            };

        public static BookingIds GetRandomBookingId(IApiService client)
        {
            List<BookingIds> bookingIds = client
                .GetMethod(ConfigurationHelper.BuildConfiguration().RestfulBooker.Resources.Booking)
                .GetValueInList<BookingIds>();
            BookingIds bookingId = bookingIds.Take(6).FirstOrDefault()!;
            return bookingId;
        }

        public static Dictionary<string, string> GenerateBookingQueryParams(IApiService client, string[] keys)
        {
            Dictionary<string, string> queryParams = new();

            BookingIds bookingId = GetRandomBookingId(client);

            ApiMethodParams apiMethodParams = new()
            {
                Uri = ConfigurationHelper.BuildConfiguration().RestfulBooker.Resources.BookingId,
                UrlSegments = new Dictionary<string, string>() { { "id", bookingId.Bookingid.ToString()! } }
            };
            Booking booking = client
                .GetMethod(apiMethodParams)
                .GetValue<Booking>();

            keys.ToList().ForEach(key =>
            {
                switch (key)
                {
                    case "firstName":
                        queryParams.Add(key, booking.Firstname);
                        break;
                    case "lastName":
                        queryParams.Add(key, booking.Lastname);
                        break;
                    case "checkin":
                        queryParams.Add(key, booking.Bookingdates.Checkin.ToString());
                        break;
                    case "checkout":
                        queryParams.Add(key, booking.Bookingdates.Checkout.ToString());
                        break;
                }
            });
            return queryParams;
        }
    }
}

