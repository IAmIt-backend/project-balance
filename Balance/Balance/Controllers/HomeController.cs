using Balance.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using System.Web.WebPages.Instrumentation;
using Balance.Models;
using MongoDB.Bson;
using System.Web.Services.Description;
using MVCModels.Models;

namespace Balance.Controllers
{
    public class HomeController : Controller
    {
        private ModelAbstractions.IService _godService = new ModelAbstractions.Service();
        [HttpGet]
        public async Task<ActionResult> Index()
        {
            return View(new IndexViewModel { Groups = await _godService.GetAllGroups() });
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
            return RedirectToAction("Index");
        }

        [HttpGet]
        public async Task<ActionResult> Group(ObjectId id)
        {
            var group = await _godService.GetGroup(id);
            var payments = await _godService.GetAllPayments(id);
            return View(new GroupViewModel {Id = id, Name = group.Name,
                Description = group.Description,
                Payments = payments,
                Sum = payments.Select(p => p.Value).Sum()
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
            _godService.AddPayment(id, model.Value, model.UserId);
            return RedirectToAction("Group", id);
        }
    }
}