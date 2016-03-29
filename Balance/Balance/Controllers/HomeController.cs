using Balance.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.WebPages.Instrumentation;
using Balance.Models;
using MongoDB.Bson;
using System.Web.Services.Description;

namespace Balance.Controllers
{
    public class HomeController : Controller
    {
        private Service _godService = new Service();
        [HttpGet]
        public ActionResult Index()
        {
            return View(new IndexViewModel { Groups = _godService.GetAllGroups() });
        }

        [HttpGet]
        public ActionResult AddGroup()
        {
            return View();
        }

        [HttpPost]
        public ActionResult AddGroup(AddGroupModel model)
        {
            _godService.AddGroup(model);
            return View();
        }

        [HttpGet]
        public ActionResult Group(ObjectId id)
        {
            var group = _godService.GetGroup(id);
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
            _godService.AddPayment(id, model.Value, model.Email);
            return View();
        }
    }
}