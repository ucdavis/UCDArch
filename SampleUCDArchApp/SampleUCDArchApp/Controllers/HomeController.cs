using UCDArch.Web.Attributes;

namespace SampleUCDArchApp.Controllers
{
    [HandleError]
    [HandleTransactionsManually]
    public class HomeController : ApplicationController
    {
        public Microsoft.AspNetCore.Mvc.ActionResult Index()
        {
            ViewData["Message"] = "Welcome to the UCDArch Sample Application!";

            return View();
        }
        
        public Microsoft.AspNetCore.Mvc.ActionResult About()
        {
            return View();
        }
    }
}