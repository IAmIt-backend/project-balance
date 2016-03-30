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
        public async Task<ActionResult> Group(string id)
        {
            var groupId = new ObjectId(id);
            var group = await _godService.GetGroup(groupId);
            return View(new GroupViewModel
            {
                Id = groupId,
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
        public async Task<ActionResult> Payment(string id, PaymentModel model)
        {
            var groupId = new ObjectId(id); 
            await _godService.AddPayment(groupId, model.Value, new ObjectId(model.UserId));
            return View();
        }
    }
}