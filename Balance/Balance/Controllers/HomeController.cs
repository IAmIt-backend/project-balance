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
        public async Task<ActionResult> AddGroup(AddGroupModel model)
        {
            if (string.IsNullOrEmpty(model.Name) || string.IsNullOrEmpty(model.Description))
            {
                ModelState.AddModelError("", "Fill all fields");
                return
                    View(new AddGroupViewModel {Description = model.Description, Name = model.Name});
            }
            else
            {
                await _godService.AddGroup(model, new ObjectId(User.Identity.GetUserId()));
                return RedirectToAction("Index");
            }
        }

        [HttpGet]
        public async Task<ActionResult> Group(ObjectId id)
        {
            var manager = HttpContext.GetOwinContext().GetUserManager<ApplicationUserManager>();
            var group = await _godService.GetGroup(id);
            var falsePayments = await _godService.GetAllPayments(id);
            var userIds = await _godService.GetAllUsersInGroup(id);
            var users = userIds.Select(l => new UserListItemModel
            {
                Id = l,
                Name = manager.FindById(l.ToString()).UserName
            }).ToList();

            var payments =
                falsePayments.Select(
                    l =>
                        new PaymentListItemModel
                        {
                            Id = l.Id,
                            Value = l.Value,
                            UserName = users.First(u => u.Id == l.Id).Name
                        }).ToList();


            return View(new GroupViewModel {Id = id, Name = group.Name,
                Description = group.Description,
                Payments = payments,
                Sum = payments.Select(p => p.Value).Sum(),
                Users = users
        });
        }


        [HttpGet]
        public ActionResult Payment()
        {
            return View();
        }

        [HttpPost]
        public async Task<ActionResult> Payment(ObjectId id ,PaymentModel model)
        {
            if (model.Value <= 0)
            {
                ModelState.AddModelError("", "Invalid value");
                return View(new PaymentViewModel { Value = model.Value });
            }
            else
            {
                var userId = new ObjectId(User.Identity.GetUserId());
                await _godService.AddPayment(id, model.Value, userId);
                return RedirectToAction("Group", "Home", new {Id = id});
            }
        }

        [HttpPost]
        public async Task<ActionResult> AddUserToGroup(string id, AddUserToGroupModel model)
        {
            var manager = HttpContext.GetOwinContext().GetUserManager<ApplicationUserManager>();
            var user = manager.FindByEmail(model.Email);
            if (string.IsNullOrEmpty(model.Email) || user == null)
            {
                ModelState.AddModelError("", "Invalid email");
                return View(new AddUserToGroupViewModel {Email = model.Email});
            }
            else if (! await _godService.IsUserAdministrator(new ObjectId(User.Identity.GetUserId()), new ObjectId(id)))
            {
                ModelState.AddModelError("", "You are not administrator");
                return View(new AddUserToGroupViewModel { Email = model.Email });
            }
            else
            {
                await _godService.AddUserToGroup(
                    new ObjectId(
                        user.Id), new ObjectId(id));
                return RedirectToAction("Group", "Home", new {Id = id});
            }
        }
        [HttpGet]
        public ActionResult AddUserToGroup()
        {
            return View();
        }
    }
}