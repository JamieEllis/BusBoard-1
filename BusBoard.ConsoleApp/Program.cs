using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;
using RestSharp;
using RestSharp.Authenticators;
using RestSharp.Deserializers;

namespace BusBoard.ConsoleApp
{
    class PostcodeAPI
    {
        public IRestResponse GetJSONFromPostcode(string postcode)
        {

            var getUrl = "/postcodes/" + postcode;
            var client = new RestClient();
            client.BaseUrl = new System.Uri("http://api.postcodes.io/");

            var request = new RestRequest();
            request.Resource = getUrl;

            return client.Execute(request);
        }
    }

    class TFLAPI
    {
        private readonly string appID;
        private readonly string appKey;

        private static HttpBasicAuthenticator authenticator;
        private static RestClient client;

        public TFLAPI(string id, string key)
        {
            appID = id;
            appKey = key;
            authenticator = new HttpBasicAuthenticator(appID, appKey);
            client = new RestClient();
            client.BaseUrl = new System.Uri("https://api.tfl.gov.uk");
            client.Authenticator = authenticator;
        }

        private IRestResponse GetJSONFromURL(string urlExtension)
        {
            var request = new RestRequest();
            request.Resource = urlExtension;
            return client.Execute(request);
        }

        public IRestResponse GetJSONFromBusStop(string busStop)
        {
            var getUrl = "/StopPoint/" + busStop + "/Arrivals";
            return GetJSONFromURL(getUrl);
        }

        public IRestResponse GetJSONFromRadLatLong(int rad, double lat, double lon)
        {   
            var getUrl = "/StopPoint?stopTypes=" + "NaptanPublicBusCoachTram" + "&radius=" + rad + "&lat=" + lat + "&lon=" + lon;
            return GetJSONFromURL(getUrl);
        }
    }

    class BusfinderSteve
    {
        public int timeToStation { get; set; }
        public string lineName { get; set; }
        public string destinationName { get; set; }
    }

    class PostcodefinderJerry
    {
        public GeolocatorHarold result = new GeolocatorHarold();
    }

    class GeolocatorHarold
    {
        public double longitude { get; set; }
        public double latitude { get; set; }
    }

    class Program
    {
        private static TFLAPI tflApi = new TFLAPI("f9d3c4a9", "b5a44afe70eb37aa5ab511614470e9d5");
        private static PostcodeAPI postcodeApi = new PostcodeAPI();
        private static JsonDeserializer jsonDeserializer = new JsonDeserializer();

        private static void PrintBusesForNaptanID(string naptanID, int numberOfBuses)
        {
            var json = tflApi.GetJSONFromBusStop(naptanID);
            var data = jsonDeserializer.Deserialize<List<BusfinderSteve>>(json);

            var recorded = new List<BusfinderSteve>();
            for (var i = 0; i < numberOfBuses; ++i)
            {
                var minItem = new BusfinderSteve
                {
                    timeToStation = 500000
                };
                foreach (var item in data)
                {
                    if (!recorded.Contains(item) && item.timeToStation <= minItem.timeToStation)
                    {
                        minItem = item;
                    }
                }
                recorded.Add(minItem);
            }

            foreach (var item in recorded)
            {
                Console.WriteLine("Bus on route {0} to {1} will arrive in {2} minutes.", item.lineName, item.destinationName, 1 + (item.timeToStation / 60));
            }
        }

        static void Main()
        {
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;

            /*
             * FIRST PART
            while (true)
            {
                Console.Write("Please enter a bus stop code: ");
                var input = Console.ReadLine();
                var json = tflApi.GetJSONFromBusStop(input); // Sounds like something you would get your husband to do while you're out at yoga
                var data = jsonDeserializer.Deserialize<List<BusfinderSteve>>(json);

                var numberOfBusesToPrint = 5;
                var recorded = new List<BusfinderSteve>();
                for (var i = 0; i < numberOfBusesToPrint; ++i)
                {
                    var minItem = new BusfinderSteve
                    {
                        timeToStation = 500000
                    };
                    foreach (var item in data)
                    {
                        if (!recorded.Contains(item) && item.timeToStation <= minItem.timeToStation)
                        {
                            minItem = item;
                        }
                    }
                    recorded.Add(minItem);
                }

                foreach (var item in recorded)
                {
                    Console.WriteLine("Bus on route {0} to {1} will arrive in {2} minutes.", item.lineName, item.destinationName, 1 + (item.timeToStation / 60));
                }
            }
        }*/

            while (true)
            {
                Console.Write("Please enter a postcode: ");
                var input = Console.ReadLine();
                var json = postcodeApi.GetJSONFromPostcode(input); 

                var resultStuff = JObject.Parse(json.Content)["result"].ToString();
                var latitude = JObject.Parse(resultStuff)["latitude"].ToObject<double>();
                var longitude = JObject.Parse(resultStuff)["longitude"].ToObject<double>();

                //  "NaptanPublicBusCoachTram",

                // Console.WriteLine("The latitude is: {0} \nThe longitude is: {1}", latitude, longitude);

                var jsonRad = tflApi.GetJSONFromRadLatLong(1000, latitude, longitude);

                Console.WriteLine("The two nearest bus stops are:");

                var resultStuffRad = JObject.Parse(jsonRad.Content)["stopPoints"];
                for (int i = 0; i < 2; ++i)
                {
                    var obj = resultStuffRad[i];
                    var naptanID = JObject.Parse(obj.ToString())["naptanId"].ToString();
                    var distance = (int)JObject.Parse(obj.ToString())["distance"];
                    Console.WriteLine("NaptanID {0} at a distance {1}m.", naptanID, distance);
                    PrintBusesForNaptanID(naptanID, 5);
                    Console.WriteLine();
                }


                //Console.WriteLine(resultStuffRad);

            }
        }
    }
}
