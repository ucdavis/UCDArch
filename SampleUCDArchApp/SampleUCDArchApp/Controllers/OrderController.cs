using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using SampleUCDArchApp.Core.Domain;
using UCDArch.Core.PersistanceSupport;
using UCDArch.Web.Attributes;
using UCDArch.Core.Utils;
using System;
using SampleUCDArchApp.Core;

namespace SampleUCDArchApp.Controllers
{
    public class OrderController : ApplicationController
    {
        private readonly IRepository<Order> _orderRepository;
        private readonly IMessageFactory _messageFactory;

        public OrderController(IRepository<Order> orderRepository, IMessageFactory messageFactory)
        {
            Check.Require(orderRepository != null);

            _orderRepository = orderRepository;
            _messageFactory = messageFactory;
        }

        //
        // GET: /Order/
        public ActionResult Index()
        {
            var orders = _orderRepository.Queryable;

            return View(orders);
        }

        public ActionResult Details(int id)
        {
            var order = _orderRepository.GetNullableById(id);

            if (order == null)
            {
                ViewBag.ErrorMessage = string.Format("No order could be found with the ID {0}", id);
                RedirectToAction("Index");
            }

            return View(order);
        }

        //
        // GET: /Order/Create

        public ActionResult Create()
        {
            var viewModel = OrderViewModel.Create(Repository.OfType<Customer>());
            viewModel.Order = new Order {OrderDate = DateTime.Now};

            return View(viewModel);
        } 

        //
        // POST: /Order/Create

        [HttpPost]
        public ActionResult Create(Order order)
        {
            if (ModelState.IsValid)
            {
                _orderRepository.EnsurePersistent(order);

                _messageFactory.SendNewOrderMessage();

                Message = "New Order Created Successfully";

                return RedirectToAction("Index");
            }
            else
            {
                _orderRepository.DbContext.RollbackTransaction();

                var viewModel = OrderViewModel.Create(Repository.OfType<Customer>());
                viewModel.Order = order;

                return View(viewModel);
            }
        }

        //
        // GET: /Order/Edit/5
 
        public ActionResult Edit(int id)
        {
            var existingOrder = _orderRepository.GetNullableById(id);

            if (existingOrder==null) return RedirectToAction("Create");

            var viewModel = OrderViewModel.Create(Repository.OfType<Customer>());

            viewModel.Order = existingOrder;
            
            return View(viewModel);
        }

        //
        // POST: /Order/Edit/5

        [HttpPost]
        public ActionResult Edit(Order order)
        {
            if (ModelState.IsValid)
            {
                _orderRepository.EnsurePersistent(order);

                Message = "Order edited successfully";

                return RedirectToAction("Index");
            }
            else
            {
                var viewModel = OrderViewModel.Create(Repository.OfType<Customer>());
                viewModel.Order = order;

                return View(viewModel);
            }
        }

        public ActionResult Delete(int id)
        {
            var order = _orderRepository.GetNullableById(id);

            if (order == null)
            {
                ViewBag.ErrorMessage = string.Format("No order could be found with the ID {0}", id);
                RedirectToAction("Index");
            }

            return View(order);
        }

        [HttpPost]
        public ActionResult Delete(Order order)
        {
            _orderRepository.Remove(order);

            Message = "Order removed successfully";
            return RedirectToAction("Index");
        }
    }

    public class OrderViewModel
    {
        public static OrderViewModel Create(IRepository<Customer> customerRepository)
        {
            Check.Require(customerRepository != null);

            var viewModel = new OrderViewModel
                                {
                                    Customers = customerRepository.Queryable.OrderBy(c => c.CompanyName).ToList()//Populate all customers
                                };

            return viewModel;
        }

        public Order Order { get; set; }
        public IList<Customer> Customers { get; set; }
    }
}
