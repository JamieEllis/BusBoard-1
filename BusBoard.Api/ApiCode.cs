using System.Collections.Generic;
using System.Net;
using Newtonsoft.Json.Linq;
using RestSharp;
using RestSharp.Authenticators;
using RestSharp.Deserializers;

namespace BusBoard.Api
{
    public class BusStationData
    {
        public string naptanId { get; set; }
        public string commonName { get; set; }
    }

    public class PostcodeAPI
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

    public class TFLAPI
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

    public class BusfinderSteve
    {
        public int timeToStation { get; set; }
        public string lineName { get; set; }
        public string destinationName { get; set; }
    }

    public class PostcodefinderJerry
    {
        public GeolocatorHarold result = new GeolocatorHarold();
    }

    public class GeolocatorHarold
    {
        public double longitude { get; set; }
        public double latitude { get; set; }
    }

    public class BadPostcodeException : System.Exception
    {
        
    }

    public class APIFun
    {
        private static TFLAPI tflApi = new TFLAPI("f9d3c4a9", "b5a44afe70eb37aa5ab511614470e9d5");
        private static PostcodeAPI postcodeApi = new PostcodeAPI();
        private static JsonDeserializer jsonDeserializer = new JsonDeserializer();

        public static GeolocatorHarold GetGeoFromPostcode(string postcode)
        {
            var json = postcodeApi.GetJSONFromPostcode(postcode);

            // Error handling.
            if (json.StatusCode != HttpStatusCode.OK)
            {
                throw new BadPostcodeException();
            }

            var resultStuff = JObject.Parse(json.Content)["result"].ToString();
            var lat = JObject.Parse(resultStuff)["latitude"].ToObject<double>();
            var lon = JObject.Parse(resultStuff)["longitude"].ToObject<double>();

            return new GeolocatorHarold
            {
                latitude = lat,
                longitude = lon
            };
        }

        public static List<BusStationData> GetBusStationDataFromGeo(GeolocatorHarold geo, int numberOfStops)
        {
            var naptanList = new List<BusStationData>();

            var jsonRad = tflApi.GetJSONFromRadLatLong(1000, geo.latitude, geo.longitude);

            var resultStuffRad = JObject.Parse(jsonRad.Content)["stopPoints"];
            for (int i = 0; i < numberOfStops; ++i)
            {
                var obj = resultStuffRad[i];
                var naptanIdStore = JObject.Parse(obj.ToString())["naptanId"].ToString();
                var commonNameStore = JObject.Parse(obj.ToString())["commonName"].ToString();
                naptanList.Add(new BusStationData { naptanId = naptanIdStore, commonName = commonNameStore});
            }

            return naptanList;
        }

        public static List<BusStationData> GetBusStationDataFromPostcode(string postcode, int numberOfStops)
        {
            var geoResult = APIFun.GetGeoFromPostcode(postcode);
            return APIFun.GetBusStationDataFromGeo(geoResult, numberOfStops);
        }

        public static List<BusfinderSteve> GetSoonestBusesForNaptanID(string naptanID, int numberOfBuses)
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

            return recorded;
        }
    }
}
