using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WatchBot.Models;

namespace WatchBot.Controllers
{
    [AllowAnonymous]
    public class GenreController : Controller
    {
        
        public ActionResult Action(string id)
        {
            return View("Genre", new DBWrapper().GetGenreViewModel("Action", int.Parse(id)));
        }
        public ActionResult Adventure(string id)
        {
            return View("Genre", new DBWrapper().GetGenreViewModel("Adventure", int.Parse(id)));
        }
        public ActionResult Animation(string id)
        {
            return View("Genre", new DBWrapper().GetGenreViewModel("Animation", int.Parse(id)));
        }
        public ActionResult Comedy(string id)
        {
            return View("Genre", new DBWrapper().GetGenreViewModel("Comedy", int.Parse(id)));
        }
        public ActionResult Documentary(string id)
        {
            return View("Genre", new DBWrapper().GetGenreViewModel("Documentary", int.Parse(id)));
        }
        public ActionResult Drama(string id)
        {
            return View("Genre", new DBWrapper().GetGenreViewModel("Drama", int.Parse(id)));
        }
        public ActionResult Family(string id)
        {
            return View("Genre", new DBWrapper().GetGenreViewModel("Family", int.Parse(id)));
        }
        public ActionResult Horror(string id)
        {
            return View("Genre", new DBWrapper().GetGenreViewModel("Horror", int.Parse(id)));
        }
        public ActionResult Romance(string id)
        {
            return View("Genre", new DBWrapper().GetGenreViewModel("Romance", int.Parse(id)));
        }
        public ActionResult ScienceFiction(string id)
        {
            return View("Genre", new DBWrapper().GetGenreViewModel("Science Fiction", int.Parse(id)));
        }
        public ActionResult Thriller(string id)
        {
            return View("Genre", new DBWrapper().GetGenreViewModel("Thriller", int.Parse(id)));
        }

    }
}