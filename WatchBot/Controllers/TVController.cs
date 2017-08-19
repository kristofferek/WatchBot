using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WatchBot.Models;

namespace WatchBot.Controllers
{
    public class TVController : Controller
    {
        // GET: TV
        public ActionResult Index()
        {
            return View(new Model().GetTvShowsViewModel());
        }

        public ActionResult TvShow(string id)
        {
            return View(new Model().GeTvShowInfoViewModel(id));
        }
    }
}