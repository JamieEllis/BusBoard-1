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
using BusBoard.Api;

namespace BusBoard.ConsoleApp
{  
    class Program
    {
        private static JsonDeserializer jsonDeserializer = new JsonDeserializer();

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

                var geoResult = APIFun.GetGeoFromPostcode(input);

                var numberOfStops = 2;

                var naptanListResult = APIFun.GetNaptanIdsFromGeo(geoResult, numberOfStops);

                Console.WriteLine("The {0} nearest bus stops are:", numberOfStops);

                foreach (var item in naptanListResult)
                {
                    Console.WriteLine("NaptanID {0}:", item);
                    APIFun.PrintBusesForNaptanID(item, 5);
                    Console.WriteLine();
                }
            }
        }
    }
}
