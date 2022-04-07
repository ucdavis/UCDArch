using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using UCDArch.Web.Attributes;

namespace SampleUCDArchApp.Controllers
{
    [HandleError]
    [HandleTransactionsManually]
    public class SampleController : ApplicationController
    {
        public Microsoft.AspNetCore.Mvc.ActionResult Index()
        {
            ViewBag.ErrorMessage = "Houston we have a problem... just kidding, this is only a test";
            Message = "Sample Message from the Controller";
            
            return View();
        }

        public Microsoft.AspNetCore.Mvc.ActionResult Form()
        {
            return View();
        }

        [HttpPost]
        public Microsoft.AspNetCore.Mvc.ActionResult Form(string data)
        {
            Message = "The form was submitted successfully";

            return View();
        }

        public Microsoft.AspNetCore.Mvc.ActionResult Table()
        {
            return View();
        }

        public Microsoft.AspNetCore.Mvc.ActionResult DataTable()
        {
            return View();
        }

        public Microsoft.AspNetCore.Mvc.ActionResult DisplayForm()
        {
            return View();
        }
    }
}
