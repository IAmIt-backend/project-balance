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
            
            if (!ModelState.IsValid)
            {
                return
                    View(new AddGroupViewModel {Description = model.Description, Name = model.Name});
            }
            else
            {
                try
                {
                    await _godService.AddGroup(model, new ObjectId(User.Identity.GetUserId()));
                }
                catch (Exception e)
                {
                    ModelState.AddModelError("", "Try to create group again");
                    return View(new AddGroupViewModel { Description = model.Description, Name = model.Name });
                }
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
            if (!ModelState.IsValid)
            {
                return View(new PaymentViewModel { Value = model.Value });
            }
            if (model.Value < 0)
            {
                ModelState.AddModelError("", "Invalid value");
                return View(new PaymentViewModel { Value = model.Value });
            }
            if (!await _godService.IsGroupActive(id))
            {
                ModelState.AddModelError("", "This group is passive. You can not add payment");
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
            if (!await _godService.IsGroupActive(new ObjectId(id)))
            {
                ModelState.AddModelError("", "This group is passive. You can not add user");
                return View(new AddUserToGroupViewModel { Email = model.Email });
            }
            if (!ModelState.IsValid)
            {
                return View(new AddUserToGroupViewModel { Email = model.Email });
            }
            var user = manager.FindByEmail(model.Email);
            if (user == null)
            {
                ModelState.AddModelError("", "Invalid email");
                return View(new AddUserToGroupViewModel { Email = model.Email });
            }
            if (! await _godService.IsUserAdministrator(new ObjectId(User.Identity.GetUserId()), new ObjectId(id)))
            {
                ModelState.AddModelError("", "You are not administrator");
                return View(new AddUserToGroupViewModel { Email = model.Email });
            }
            else
            {
                try
                {
                    await _godService.AddUserToGroup(
                        new ObjectId(
                            user.Id), new ObjectId(id));
                }
                catch (Exception e)
                {
                    ModelState.AddModelError("", e.Message);
                    return View(new AddUserToGroupViewModel { Email = model.Email });
                }
                return RedirectToAction("Group", "Home", new {Id = id});
            }
        }
        [HttpGet]
        public ActionResult AddUserToGroup()
        {
            return View();
        }

        [HttpGet]
        public async Task<ActionResult> Count(string id)
        {
            var payments = (await _godService.GetAllPayments(new ObjectId(id))).GroupBy(u => u.Id);

            var values = payments.Select(g => new CountListItemModel {Id = g.Key, Value = g.ToList().Select(a => a.Value).Sum()}).ToList();
            var constant = values.Select(v => v.Value).Sum()/values.Count();
            var newValues = values.Select(v => new CountListItemModel {Id = v.Id, Value = v.Value - constant}).ToList();

            var minuses = new Queue<CountListItemModel>(newValues.Where(v => v.Value < 0));
            var pluses = new Queue<CountListItemModel>(newValues.Where(v => v.Value > 0));

            var zeroes = newValues.Where(v => v.Value == 0).ToList();
            var viewModel = new CountViewModel { Id = id, Credits = new List<CreditModel>(), Type = ""};
            if (zeroes.Select(z => z.Id).Contains(new ObjectId(User.Identity.GetUserId())))
            {
                return View(viewModel);
            }
            var currentUser = minuses.FirstOrDefault(m => m.Id == new ObjectId(User.Identity.GetUserId()));
            if (currentUser != null)
            {
                //he is debtor
                while (minuses.Peek() != currentUser)
                {
                    if (Math.Abs(minuses.Peek().Value) > pluses.Peek().Value)
                    {
                        minuses.Peek().Value += pluses.Peek().Value;
                        pluses.Dequeue();
                    }
                    else if (Math.Abs(minuses.Peek().Value) < pluses.Peek().Value)
                    {
                        pluses.Peek().Value += minuses.Peek().Value;
                        minuses.Dequeue();
                    }
                    else if (Math.Abs(minuses.Peek().Value) == pluses.Peek().Value)
                    {
                        minuses.Dequeue();
                        pluses.Dequeue();
                    }
                }
                var manager = HttpContext.GetOwinContext().GetUserManager<ApplicationUserManager>();
                while (minuses.Peek().Value < 0)
                {
                    if (Math.Abs(minuses.Peek().Value) >= pluses.Peek().Value)
                    {
                        viewModel.Credits.Add(new CreditModel
                        {
                            Credit = pluses.Peek().Value,
                            Name = manager.FindById(pluses.Peek().Id.ToString()).UserName
                        });
                        minuses.Peek().Value += pluses.Peek().Value;
                        pluses.Dequeue();
                    }
                    else if (Math.Abs(minuses.Peek().Value) < pluses.Peek().Value)
                    {
                        viewModel.Credits.Add(new CreditModel
                        {
                            Credit = Math.Abs(minuses.Peek().Value),
                            Name = manager.FindById(pluses.Peek().Id.ToString()).UserName
                        });
                        minuses.Peek().Value += pluses.Peek().Value;
                    }
                }
                viewModel.Type = "Your credits";
            }
            else
            {
                {
                    //he is creditor
                    var currentPlusUser = pluses.FirstOrDefault(m => m.Id == new ObjectId(User.Identity.GetUserId()));
                    while (pluses.Peek().Id != currentPlusUser.Id)
                    {
                        if (Math.Abs(minuses.Peek().Value) > pluses.Peek().Value)
                        {
                            minuses.Peek().Value += pluses.Peek().Value;
                            pluses.Dequeue();
                        }
                        else if (Math.Abs(minuses.Peek().Value) < pluses.Peek().Value)
                        {
                            pluses.Peek().Value += minuses.Peek().Value;
                            minuses.Dequeue();
                        }
                        else if (Math.Abs(minuses.Peek().Value) == pluses.Peek().Value)
                        {
                            minuses.Dequeue();
                            pluses.Dequeue();
                        }
                    }
                    var manager = HttpContext.GetOwinContext().GetUserManager<ApplicationUserManager>();
                    while (pluses.Peek().Value > 0)
                    {
                        if (pluses.Peek().Value >= Math.Abs(minuses.Peek().Value))
                        {
                            viewModel.Credits.Add(new CreditModel
                            {
                                Credit = Math.Abs(minuses.Peek().Value),
                                Name = manager.FindById(minuses.Peek().Id.ToString()).UserName
                            });
                            pluses.Peek().Value += minuses.Peek().Value;
                            minuses.Dequeue();
                        }
                        else if (pluses.Peek().Value < Math.Abs(minuses.Peek().Value))
                        {
                            viewModel.Credits.Add(new CreditModel
                            {
                                Credit = pluses.Peek().Value,
                                Name = manager.FindById(minuses.Peek().Id.ToString()).UserName
                            });
                            pluses.Peek().Value += minuses.Peek().Value;
                        }
                    }
                    viewModel.Type = "Your creditors";
                }
            }
            //await _godService.SetGroupState(new ObjectId(id));
            return View(viewModel);
        }
    }
}