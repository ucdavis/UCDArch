using System.Linq;
using System.Web.Mvc;
using UCDArch.Web.Attributes;
using UCDArch.Web.Controller;
using SampleUCDArchApp.Core.Domain;
using UCDArch.Web.ActionResults;

namespace SampleUCDArchApp.Controllers
{
    public class JsonController : ApplicationController
    {
        //
        // GET: /Json/
        [HandleTransactionsManually]
        public ActionResult Index()
        {
            Message = "Go to /Get/ to get an Order, or /List/ to a list of Orders, all in Json";

            return View();
        }

        /// <summary>
        /// Returns a Json order and uses ISO serialization for the date
        /// </summary>
        public ActionResult Get()
        {
            var order = Repository.OfType<Order>().Queryable.First();

            return new JsonNetResult(order, JsonDateConversionStrategy.Iso);
        }

        /// <summary>
        /// Returns a Json list of orders 
        /// </summary>
        public ActionResult List()
        {
            var orders = Repository.OfType<Order>().Queryable.OrderBy(o => o.OrderDate).Take(100).ToList();
            
            return new JsonNetResult(orders);
        }
    }
}
