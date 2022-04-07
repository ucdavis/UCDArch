using System.Linq;
using UCDArch.Web.Controller;
using SampleUCDArchApp.Core.Domain;
using UCDArch.Core.PersistanceSupport;

namespace SampleUCDArchApp.Controllers
{
    public class CustomerController : ApplicationController
    {
        private readonly IRepository<Customer> _customers;

        public CustomerController(IRepository<Customer> customers)
        {
            _customers = customers;
        }

        public Microsoft.AspNetCore.Mvc.ActionResult Index()
        {
            var customers = _customers.Queryable;

            return View(customers.ToList());
        }
    }
}
