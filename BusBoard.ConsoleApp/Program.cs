using System;
using System.Net;
using RestSharp.Deserializers;
using BusBoard.Api;

namespace BusBoard.ConsoleApp
{  
    public class Program
    {
        private static JsonDeserializer jsonDeserializer = new JsonDeserializer();

/*        public static string OutputFromPostcode(string postcode, int numberOfStops)
        {
            var geoResult = APIFun.GetGeoFromPostcode(postcode);

            var naptanListResult = APIFun.GetNaptanIdsFromGeo(geoResult, numberOfStops);

            var temp = "";

            temp += "The " + numberOfStops + " nearest bus stops are:\n\n";

            foreach (var item in naptanListResult)
            {
                temp += "NaptanID " + item + ":\n";
                temp += APIFun.PrintBusesForNaptanID(item, 5) + "\n";
            }

            return temp;
        }*/

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

            /*
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
            */
            
            /*
            while (true)
            {
                Console.Write("Please enter a postcode: ");
                var input = Console.ReadLine();
                Console.WriteLine(APIFun.OutputFromPostcode(input, 2));
            }
            */
        }
    }
}
