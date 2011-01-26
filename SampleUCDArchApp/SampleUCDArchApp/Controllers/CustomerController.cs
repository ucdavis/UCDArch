using System.Web.Mvc;
using UCDArch.Web.Controller;
using SampleUCDArchApp.Core.Domain;

namespace SampleUCDArchApp.Controllers
{
    public class CustomerController : SuperController
    {
        public ActionResult Index()
        {
            var customers = Repository.OfType<Customer>().GetAll();

            return View(customers);
        }
    }
}
