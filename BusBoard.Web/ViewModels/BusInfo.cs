using System.Collections.Generic;

namespace BusBoard.Web.ViewModels
{
  public class BusInfo
  {
    public BusInfo(string postCode)
    {
      PostCode = postCode;
      TableData = new List<WebpageTable>();
    }

    public string PostCode { get; set; }
    public List<WebpageTable> TableData { get; set; }
  }
}