using System.Web.Mvc;
using UCDArch.Web.Attributes;

namespace SampleUCDArchApp.Controllers
{
    [HandleError]
    [HandleTransactionsManually]
    public class HomeController : ApplicationController
    {
        public ActionResult Index()
        {
            ViewData["Message"] = "Complete seperate sample app dummy!";

            return View();
        }
        
        public ActionResult About()
        {
            return View();
        }
    }
}