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
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using MVCModels.Models;

namespace Balance.Controllers
{
    [Authorize]
    public class HomeController : Controller
    {
        private ModelAbstractions.IService _godService = new ModelAbstractions.Service();
        [HttpGet]
        public async Task<ActionResult> Index()
        {
            return View(new IndexViewModel { Groups = await _godService.GetAllGroupsOfUser(new ObjectId(User.Identity.GetUserId())) });
        }

        [HttpGet]
        public ActionResult AddGroup()
        {
            return View();
        }

        [HttpPost]
        public ActionResult AddGroup(AddGroupModel model)
        {
            if (string.IsNullOrEmpty(model.Name) || model.Description == null)
            {
                ModelState.AddModelError("", "Fill all fields");
                return
                    View(new AddGroupViewModel {Description = model.Description, Name = model.Name});
            }
            else
            {
                _godService.AddGroup(model, new ObjectId(User.Identity.GetUserId()));
                return RedirectToAction("Index");
            }
        }

        [HttpGet]
        public async Task<ActionResult> Group(ObjectId id)
        {
            var manager = HttpContext.GetOwinContext().GetUserManager<ApplicationUserManager>();
            var group = await _godService.GetGroup(id);
            var falsePayments = await _godService.GetAllPayments(id);
            var users = await _godService.GetAllUsersInGroup(id);
            var payments =
                falsePayments.Select(
                    l =>
                        new PaymentListItemModel
                        {
                            Id = l.Id,
                            Value = l.Value,
                            UserName = manager.FindById(l.Id.ToString()).UserName
                        }).ToList();

            return View(new GroupViewModel {Id = id, Name = group.Name,
                Description = group.Description,
                Payments = payments,
                Sum = payments.Select(p => p.Value).Sum(),
                Users = users.Select(
                l => new UserListItemModel { Id = l.Id, Name = manager.FindById(l.Id.ToString()).UserName }).ToList()
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
            if (model.Value == default(decimal) || model.Value < 0)
            {
                ModelState.AddModelError("", "Invalid value");
                return View(new PaymentViewModel {});
            }
            else
            {
                var userId = new ObjectId(User.Identity.GetUserId());
                _godService.AddPayment(id, model.Value, userId);
                return RedirectToAction("Group", id);
            }
        }

        [HttpPost]
        public async Task<ActionResult> AddUserToGroup(string id, AddUserToGroupModel model)
        {
            var manager = HttpContext.GetOwinContext().GetUserManager<ApplicationUserManager>();
            if (string.IsNullOrEmpty(model.Email) || string.IsNullOrEmpty(User.Identity.GetUserName()))
            {
                ModelState.AddModelError("", "Invalid email");
                return View(new AddUserToGroupViewModel {Email = model.Email});
            }
            else if (! await _godService.IsUserAdministrator(new ObjectId(manager.FindByEmail(model.Email).Id), new ObjectId(id)))
            {
                ModelState.AddModelError("", "You are not administrator");
                return View(new AddUserToGroupViewModel { Email = model.Email });
            }
            else
            {
                await _godService.AddUserToGroup(
                    new ObjectId(
                        HttpContext.GetOwinContext()
                            .GetUserManager<ApplicationUserManager>()
                            .FindByEmail(model.Email)
                            .Id), new ObjectId(id));
                return RedirectToAction("Group", id);
            }
        }
    }
}