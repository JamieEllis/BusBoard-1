using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BusBoard.Web.ViewModels
{
    public class WebpageTableLine
    {
        public string lineName { get; set; }
        public string destinationName { get; set; }
        public string arrivalMessage { get; set; }
    }

    public class WebpageTable
    {
        public string title { get; set; }
        public List<WebpageTableLine> data { get; set; }
    }
}