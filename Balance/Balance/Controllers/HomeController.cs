using Balance.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.WebPages.Instrumentation;
using MongoDB.Bson;
using System.Web.Services.Description;
using MVCModels.Models;
using ModelAbstractions;
using System.Threading.Tasks;

namespace Balance.Controllers
{
    public class HomeController : Controller
    {
        private ModelAbstractions.Service _godService = new ModelAbstractions.Service();
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
        public async Task<ActionResult> AddGroup(AddGroupModel model)
        {
            await _godService.AddGroup(model);
            return View();
        }

        [HttpGet]
        public async Task<ActionResult> Group(string groupId)
        {
            var id = new ObjectId(groupId);
            var group = await _godService.GetGroup(id);
            return View(new GroupViewModel
            {
                Id = id,
                Name = group.Name,
                Description = group.Description
            });
        }


        [HttpGet]
        public ActionResult Payment()
        {
            return View();
        }

        [HttpPost]
        public async Task<ActionResult> Payment(ObjectId id, PaymentModel model)
        {
            await _godService.AddPayment(id, model.Value, new ObjectId(model.UserId));
            return View();
        }
    }
}