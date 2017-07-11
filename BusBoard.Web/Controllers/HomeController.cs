using System.Collections.Generic;
using System.Web.Mvc;
using BusBoard.Api;
using BusBoard.Web.Models;
using BusBoard.Web.ViewModels;

namespace BusBoard.Web.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index(string errorMessage = "")
        {
            ViewBag.Message = errorMessage;
            return View();
        }

        [HttpGet]
        public ActionResult BusInfo(PostcodeSelection selection)
        {
            // Add some properties to the BusInfo view model with the data you want to render on the page.
            // Write code here to populate the view model with info from the APIs.
            // Then modify the view (in Views/Home/BusInfo.cshtml) to render upcoming buses.
            var webpageTables = new List<WebpageTable>();

            List<BusStationData> busStationData;
            try
            {
               busStationData = APIFun.GetBusStationDataFromPostcode(selection.Postcode, 2);
            }
            catch (BadPostcodeException except)
            {
                return RedirectToAction("Index", new { errorMessage = $"That postcode ({selection.Postcode}) is not valid. Please enter another." });
            }

            foreach (var busStation in busStationData)
            {
                var newWebpageTable = new WebpageTable();
                newWebpageTable.data = new List<WebpageTableLine>();
                newWebpageTable.title = busStation.commonName;
                var busData = APIFun.GetSoonestBusesForNaptanID(busStation.naptanId, 5);
                foreach (var bus in busData)
                {
                    var newWebpageTableLine = new WebpageTableLine();
                    newWebpageTableLine.lineName = bus.lineName;
                    newWebpageTableLine.destinationName = bus.destinationName;
                    var time = bus.timeToStation / 60;
                    if (time == 0)
                    {
                        newWebpageTableLine.arrivalMessage = "Due";
                    }
                    else if (time == 1)
                    {
                        newWebpageTableLine.arrivalMessage = "1 min";
                    }
                    else
                    {
                        newWebpageTableLine.arrivalMessage = $"{time} mins";
                    }
                    newWebpageTable.data.Add(newWebpageTableLine);
                }
                webpageTables.Add(newWebpageTable);
            }

            var info = new BusInfo(selection.Postcode)
            {
                TableData = webpageTables
            };

            return View(info);
        }

        public ActionResult About()
        {
            ViewBag.Message = "Information about this site";

            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Contact us!";

            return View();
        }
    }
}