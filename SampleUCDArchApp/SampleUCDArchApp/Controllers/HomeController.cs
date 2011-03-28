using System.Web.Mvc;
using UCDArch.Web.Attributes;
using UCDArch.Web.Controller;

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

        public ViewResult Sample()
        {
            Message = "Here is a status message stored in TempData";
            return View();
        }

        public ActionResult About()
        {
            return View();
        }
    }
}
