using Balance.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Balance.Models;

namespace Balance.Controllers
{
    public class HomeController : Controller
    {
        [HttpPost]
        public ActionResult Index(IndexModel model)
        {
            var totalMessage = "";
                var text = model.text;
                var total = "";
                for (int t = text.Length - 1; t >= 0; t--)
                {
                    total += text.ElementAt(t);
                }
                totalMessage = total;
            return View(new IndexViewModel { text = totalMessage });
        }

        [HttpGet]
        public ActionResult Index()
        {
            var totalMessage = "";
            return View(new IndexViewModel { text = totalMessage });
        }

        public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";

            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }
    }
}