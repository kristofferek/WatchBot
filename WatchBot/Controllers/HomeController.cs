using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WatchBot.Models;
using WatchBot.Models.ViewModels;

namespace WatchBot.Controllers
{
    [AllowAnonymous]
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            DBWrapper movieDB = new DBWrapper();
            DiscoverViewModel discover = movieDB.GetDiscoverViewModel();
            return View(discover);
        }

        public ActionResult Movie(string id)
        {
            return View(new DBWrapper().GetMovie(Int32.Parse(id)));
        }

        public ActionResult Genre(string genre)
        {
            return View();
        }

        [HttpPost]
        public ActionResult Contact(String message)
        {
            ViewBag.TheMessage = "Thanks, we got your message.";

            return View();
        }

        public ActionResult About()
        {
            return View("About");
        }
    }
}