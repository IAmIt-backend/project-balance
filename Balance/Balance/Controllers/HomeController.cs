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
using Converter;
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
        public async Task<ActionResult> Invitations() {
            var userId = new ObjectId(User.Identity.GetUserId());
            var viewModel = new InvitationsViewModel { ItemModels = await _godService.GetAllInvitations(userId) };
            return View(viewModel);
        }
        [HttpPost]
        public async Task<ActionResult> Invitations(ICollection<InvitationListItemModel> models)
        {
            if (models != null)
            {
                var userId = new ObjectId(User.Identity.GetUserId());
                foreach (var invitation in models)
                {
                    if (invitation.IsVerified && !invitation.IsRejected)
                        await _godService.VerifyInvitation(userId, invitation.GroupId);
                    else if (invitation.IsRejected && !invitation.IsVerified)
                        await _godService.RejectInvitation(userId, invitation.GroupId);
                }
            }
            return RedirectToAction("Index");
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
                falsePayments.Select(l =>
                    new PaymentListItemModel
                    {
                        Id = l.Id,
                        Value = l.Value,
                        UserName = users.First(u => u.Id == l.Id).Name,
                        Type = l.Type
                    });

            var newPayments = new List<PaymentListItemModel>(payments);
            return View(new GroupViewModel
            {
                Id = id,
                Name = group.Name,
                Description = group.Description,
                Payments = newPayments,
                Users = users
            });
        }

        [HttpGet]
        public ActionResult Payment()
        {
                var userId = new ObjectId(User.Identity.GetUserId());
                return View(new PaymentViewModel {Types = new List<string> { CurrencyType.USD, CurrencyType.CNY, CurrencyType.EUR, CurrencyType.GBP, CurrencyType.JPY, CurrencyType.PLN, CurrencyType.RUB } });
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
                await _godService.AddPayment(id, model.Value, userId, model.Type);
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
            var newPayments = await Task.WhenAll((await _godService.GetAllPayments(new ObjectId(id))).Select(async g => new PaymentListItemModel
            {
                Id = g.Id,
                UserName = g.UserName,
                Value = await Converter.Converter.Convert(g.Type, CurrencyType.USD, g.Value),
                Type = CurrencyType.USD
            }));

            var payments = new List<PaymentListItemModel>(newPayments).GroupBy(u => u.Id);
            var falseValues = payments.Select(g => new CountListItemModel
            {
                Id = g.Key,
                Value = g.ToList().Select(a => a.Value).Sum()
            });
            var values = new List<CountListItemModel>(falseValues);

            var constant = values.Select(v => v.Value).Sum()/values.Count();
            var newValues = values.Select(v => new CountListItemModel {Id = v.Id, Value = v.Value - constant}).ToList();

            var minuses = new Queue<CountListItemModel>(newValues.Where(v => v.Value < 0));
            var pluses = new Queue<CountListItemModel>(newValues.Where(v => v.Value > 0));

            var zeroes = newValues.Where(v => v.Value == 0).ToList();
            var viewModel = new CountViewModel { Id = id, Credits = new List<CreditModel>(), Type = "", Types = new List<string>
            {
                 CurrencyType.USD, CurrencyType.CNY, CurrencyType.EUR, CurrencyType.GBP, CurrencyType.JPY, CurrencyType.PLN, CurrencyType.RUB
            } };
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
                while (minuses.Peek().Value <= (decimal) -0.00001)
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
                    while (pluses.Peek().Value >= (decimal) 0.00001)
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
        
        
        [HttpPost]
        public async Task<ActionResult> Count(string id, CountViewModel model)
        {
            var newPayments = await Task.WhenAll((await _godService.GetAllPayments(new ObjectId(id))).Select(async g => new PaymentListItemModel
            {
                Id = g.Id,
                UserName = g.UserName,
                Value = await Converter.Converter.Convert(g.Type, model.Type, g.Value),
                Type = CurrencyType.USD
            }));

            var payments = new List<PaymentListItemModel>(newPayments).GroupBy(u => u.Id);
            var falseValues = payments.Select(g => new CountListItemModel
            {
                Id = g.Key,
                Value = g.ToList().Select(a => a.Value).Sum()
            });
            var values = new List<CountListItemModel>(falseValues);

            var constant = values.Select(v => v.Value).Sum() / values.Count();
            var newValues = values.Select(v => new CountListItemModel { Id = v.Id, Value = v.Value - constant }).ToList();

            var minuses = new Queue<CountListItemModel>(newValues.Where(v => v.Value < 0));
            var pluses = new Queue<CountListItemModel>(newValues.Where(v => v.Value > 0));

            var zeroes = newValues.Where(v => v.Value == 0).ToList();
            var viewModel = new CountViewModel
            {
                Id = id,
                Credits = new List<CreditModel>(),
                Type = "",
                Types = new List<string>
            {
                 CurrencyType.USD, CurrencyType.CNY, CurrencyType.EUR, CurrencyType.GBP, CurrencyType.JPY, CurrencyType.PLN, CurrencyType.RUB
            }
            };
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
                while (minuses.Peek().Value <= (decimal) -0.00001)
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
                    while (pluses.Peek().Value >= (decimal) 0.00001)
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