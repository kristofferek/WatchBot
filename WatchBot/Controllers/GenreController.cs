using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WatchBot.Models;

namespace WatchBot.Controllers
{
    public class GenreController : Controller
    {
        
        public ActionResult Action(string id)
        {
            return View("Genre", new Model().GetGenreViewModel("Action", int.Parse(id)));
        }
        public ActionResult Adventure(string id)
        {
            return View("Genre", new Model().GetGenreViewModel("Adventure", int.Parse(id)));
        }
        public ActionResult Animation(string id)
        {
            return View("Genre", new Model().GetGenreViewModel("Animation", int.Parse(id)));
        }
        public ActionResult Comedy(string id)
        {
            return View("Genre", new Model().GetGenreViewModel("Comedy", int.Parse(id)));
        }
        public ActionResult Documentary(string id)
        {
            return View("Genre", new Model().GetGenreViewModel("Documentary", int.Parse(id)));
        }
        public ActionResult Drama(string id)
        {
            return View("Genre", new Model().GetGenreViewModel("Drama", int.Parse(id)));
        }
        public ActionResult Family(string id)
        {
            return View("Genre", new Model().GetGenreViewModel("Family", int.Parse(id)));
        }
        public ActionResult Horror(string id)
        {
            return View("Genre", new Model().GetGenreViewModel("Horror", int.Parse(id)));
        }
        public ActionResult Romance(string id)
        {
            return View("Genre", new Model().GetGenreViewModel("Romance", int.Parse(id)));
        }
        public ActionResult ScienceFiction(string id)
        {
            return View("Genre", new Model().GetGenreViewModel("Science Fiction", int.Parse(id)));
        }
        public ActionResult Thriller(string id)
        {
            return View("Genre", new Model().GetGenreViewModel("Thriller", int.Parse(id)));
        }

    }
}