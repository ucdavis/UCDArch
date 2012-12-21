using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using UCDArch.Web.Attributes;

namespace $rootnamespace$.Controllers
{
    [HandleError]
    [HandleTransactionsManually]
    public class SampleController : ApplicationController
    {
        public ActionResult Index()
        {
            ViewBag.ErrorMessage = "Houston we have a problem... just kidding, this is only a test";
            Message = "Sample Message from the Controller";
            
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
    }
}
