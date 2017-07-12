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
            return View(new DBWrapper().GetDetailsViewModel(id));
        }
    }
}