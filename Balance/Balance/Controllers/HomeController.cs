using Balance.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.WebPages.Instrumentation;
using Balance.Models;
using MongoDB.Bson;

namespace Balance.Controllers
{
    public class HomeController : Controller
    {
        private IService _service = new IService();
        [HttpGet]
        public ActionResult Index()
        {
            return View(new IndexViewModel { Groups = _service.GetAllGroups() });
        }

        [HttpGet]
        public ActionResult AddGroup()
        {
            return View();
        }

        [HttpPost]
        public ActionResult AddGroup(AddGroupModel model)
        {
            _service.AddGroup(model);
            return View();
        }

        [HttpGet]
        public ActionResult Group(ObjectId id)
        {
            var group = _service.GetGroup(id);
            return View(new GroupViewModel {Id = id, Name = group.Name(),
                Description = group.Description(),
                Sum = group.Payments.Select(p => p.Value).Sum()
        });
        }


        [HttpGet]
        public ActionResult Payment()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Payment(ObjectId id ,PaymentModel model)
        {
            _service.AddPayment(id, model.Value, model.Email);
            return View();
        }
    }
}