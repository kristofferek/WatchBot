using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.ModelBinding;
using System.Web.WebPages;
using Colors;
using Newtonsoft.Json.Linq;
using WatchBot.Models;

namespace WatchBot.Models
{
    public class DBWrapper
    {
        private const string API_KEY = "78a3dcb0adbc04e0efe7af4db390f544";
        private const string BASE_URL = "https://api.themoviedb.org/3/";

        private ICollection<MovieViewModel> popular = new List<MovieViewModel>();
        private ICollection<MovieViewModel> topRated = new List<MovieViewModel>();

        private ICollection<Genre> genres;

        private DiscoverViewModel discoverViewModel;

        public MovieViewModel GetMovie(int id)
        {
            foreach (var movie in popular)
            {
                if (movie.Id != id) continue;
                movie.Actors = GetActors(id);
                return movie;
            }
            foreach (var movie in topRated)
            {
                if (movie.Id != id) continue;
                movie.Actors = GetActors(id);
                return movie;
            }
            string json;
            using (WebClient wc = new WebClient())
            {
                json = wc.DownloadString(BASE_URL + "movie/"+ id 
                    +"?api_key=" + API_KEY + "&language=en-US&include_video=false");
            }
            JObject o = JObject.Parse(json);
            MovieViewModel m = GetMovieFromJObject(o);
            m.Actors = GetActors(id);
            return m;
        }

        private MovieViewModel GetMovieFromJObject(JObject o)
        {
            MovieViewModel movie = new MovieViewModel();
            movie.Id = (int)o["id"];
            movie.Title = (string)o["title"];
            movie.Backdrop = "https://image.tmdb.org/t/p/w1280" + (string)o["backdrop_path"];
            movie.Description = (string)o["overview"];

            try
            {
                JArray genArray = JArray.FromObject(o["genres"]);
                for (int i = 0; i < genArray.Count; i++)
                {
                    JObject genre = JObject.FromObject(genArray[i]);
                    movie.Genres.Add(new Genre((int)genre["id"], (string)genre["name"]));
                }
            }
            catch (ArgumentNullException e)
            {
                Console.WriteLine(e);
            }
            
            movie.ImdbID = (string)o["imdb_id"];
            movie.Poster = "https://image.tmdb.org/t/p/w500" + (string)o["poster_path"];
            movie.Rating = (double)o["vote_average"];
            movie.ReleaseDate = (string)o["release_date"];
            try
            {
                movie.Runtime = (int) o["runtime"];
            }
            catch (ArgumentNullException e)
            {
                Console.WriteLine(movie.Title);
                Console.WriteLine(e);
            }
            //movie.Actors = GetActors(movie.Id);
            movie.ProminentColor = Utils.Utils.HexConverter(
                    ColorMath.getDominantColor(
                        GetBitmapFromUrl("https://image.tmdb.org/t/p/w92" + 
                        (string) o["poster_path"])));

            return movie;
        }

        public ICollection<MovieViewModel> GetPopularList(int amount)
        {
            if (popular.Count > 0)
            {
                return popular;
            }
            var query = "&language=en-US&sort_by=popularity.desc&include_adult=false&include_video=false&page=1&vote_count.gte=200&with_original_language=en";
            return GetList(amount, query, popular);
        }

        public ICollection<MovieViewModel> GetTopRatedList(int amount)
        {
            if (topRated.Count > 0)
            {
                return topRated;
            }
            var query = "&language=en-US&sort_by=vote_average.desc&include_adult=false&include_video=false&page=1&vote_count.gte=200&with_original_language=en";
            return GetList(amount, query, topRated);
        }

        private ICollection<MovieViewModel> GetList(int amount, string query, ICollection<MovieViewModel> movies)
        {
            string json;
            using (WebClient wc = new WebClient())
            {
                json = wc.DownloadString(BASE_URL + "discover/movie"
                    + "?api_key=" + API_KEY + query);
            }
            JObject o = JObject.Parse(json);
            JArray results = JArray.FromObject(o["results"]);

            for (int i = 0; i < amount && i < results.Count; i++)
            {
                JObject movie = JObject.FromObject(results[i]);
                movies.Add(GetMovieFromJObject(movie));
            }
            return movies;

        }

        private List<ActorViewModel> GetActors(int movieId)
        {
            List<ActorViewModel> actorList = new List<ActorViewModel>();
            string json;
            using (WebClient wc = new WebClient())
            {
                json = wc.DownloadString(BASE_URL + "movie/" + movieId + "/credits"
                    + "?api_key=" + API_KEY);
            }
            var o = JObject.Parse(json);
            var actors = JArray.FromObject(o["cast"]);
            for (int i = 0; i < 5 && i < actors.Count; i++)
            {
                var actorObj = JObject.FromObject(actors[i]);
                ActorViewModel actor = new ActorViewModel();
                actor.Name = (string) actorObj["name"];
                actor.MovieName = (string) actorObj["character"];
                actor.Picture = "https://image.tmdb.org/t/p/w500" + (string) actorObj["profile_path"];
                actorList.Add(actor);
            }
            return actorList;
        }

        private ICollection<Genre> GetGenres()
        {
            if (genres != null)
            {
                return genres;
            }
            genres = new List<Genre>();
            string json;
            using (WebClient wc = new WebClient())
            {
                json = wc.DownloadString(BASE_URL + "genre/movie/list?api_key=" + API_KEY + "&language=en-US");
            }
            var o = JObject.Parse(json);
            var results = JArray.FromObject(o["genres"]);

            foreach (var t in results)
            {
                var genre = JObject.FromObject(t);

                genres.Add(new Genre((int)genre["id"], (string)genre["name"]));
            }
            return genres;
        }

        private static Bitmap GetBitmapFromUrl(string src)
        {
            var request = WebRequest.Create(src);
            var response = request.GetResponse();
            var responseStream = response.GetResponseStream();
            var bitmap = new Bitmap(responseStream);
            return bitmap;
        }

        private void SetFeatureMovies(DiscoverViewModel dvb)
        {
            if (dvb == null) return;
            Random r = new Random();
            int index = r.Next(dvb.Popular.Count - 1);
            dvb.FeaturePopular = dvb.Popular.ElementAt(index);
            dvb.Popular.Remove(dvb.FeaturePopular);

            index = r.Next(dvb.TopRated.Count - 1);
            dvb.FeatureTopRated = dvb.TopRated.ElementAt(index);
            dvb.TopRated.Remove(dvb.FeatureTopRated);
        }

        public DiscoverViewModel GetDiscoverViewModel()
        {
            discoverViewModel = HttpContext.Current.Session["Discover"] as DiscoverViewModel;
            if (discoverViewModel != null)
            {
                return discoverViewModel;
            }
            discoverViewModel = new DiscoverViewModel
            {
                Popular = GetPopularList(21),
                TopRated = GetTopRatedList(21)
            };
            SetFeatureMovies(discoverViewModel);
            HttpContext.Current.Session["Discover"] = discoverViewModel;
            return discoverViewModel;

        }
    }
}