using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WatchBot.Models;
using WatchBot.Models.ViewModels;

namespace WatchBot.Controllers
{
    public class HomeController : Controller
    {
        [AllowAnonymous]
        public ActionResult Index()
        {
            Model movieDB = new Model();
            DiscoverViewModel discover = movieDB.GetDiscoverViewModel();
            return View(discover);
        }

        public ActionResult Movie(string id)
        {
            return View(new Model().GetMovieInfoViewModel(id));
        }
    }
}