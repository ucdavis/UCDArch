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
            ViewData["Message"] = "Welcome to the UCDArch Sample Application!";

            return View();
        }
        
        public ActionResult About()
        {
            return View();
        }
    }
}