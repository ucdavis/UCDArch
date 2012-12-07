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

        public ViewResult Sample()
        {
            ViewBag.ErrorMessage = "Houston we have a problem... just kidding, this is only a test";
            Message = "Sample Message from the Controller";
            return View();
        }

        public ActionResult Full()
        {
            Message = "Here is a status message stored in TempData";
            return View();
        }

        public ActionResult Form()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Form(string data)
        {
            Message = "The form was submitted successfully";

            return View();
        }

        public ActionResult Table()
        {
            return View();
        }

        public ActionResult DataTable()
        {
            return View();
        }

        public ActionResult DisplayForm()
        {
            return View();
        }

        public ViewResult Placeholder()
        {
            return View();
        }

        public ActionResult About()
        {
            return View();
        }
    }
}