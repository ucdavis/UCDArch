using System.Web.Mvc;
using SampleUCDArchApp.Core.Domain;
using UCDArch.Core.PersistanceSupport;
using UCDArch.Web.Controller;
using UCDArch.Web.Attributes;
using UCDArch.Core.Utils;


namespace SampleUCDArchApp.Controllers
{
    public class ProductController : SuperController
    {
        private readonly IRepository<Product> _productRepository;

        public ProductController(IRepository<Product> productRepository)
        {
            Check.Require(productRepository != null);
            _productRepository = productRepository;
        }

        // GET: /Product/
        [HandleTransactionsManually]
        public ActionResult Index()
        {
            var products = _productRepository.Queryable;
            return View(products);
        }

        // Create: /Product/
        public ActionResult Create()
        {
            var viewModel = ProductViewModel.Create();
            return View(viewModel);
        }
    }

    public class ProductViewModel
    {
        public static ProductViewModel Create()
        {
            var viewModel = new ProductViewModel();
            return viewModel;
        }

        public Product Product { get; set; }

    }
}
