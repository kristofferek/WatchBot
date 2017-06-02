using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Net;
using System.Text;
using System.Web;
using Newtonsoft.Json.Linq;
using WatchBot.Models.ViewModels;

namespace WatchBot.Models
{
    public class DBWrapper
    {
        private const string API_KEY = "78a3dcb0adbc04e0efe7af4db390f544";
        private const string BASE_URL = "https://api.themoviedb.org/3/";

        private Dictionary<string, int> _genres;

        public MovieViewModel GetMovie(int id)
        {
            var dvm = HttpContext.Current.Session["Discover"] as DiscoverViewModel;
            if (dvm != null)
            {
                var movie = dvm.TopRated.ContainsKey(id) ? dvm.TopRated[id] : null;
                movie = dvm.TopRated.ContainsKey(id) && movie != null ? dvm.TopRated[id] : null;
                if (movie != null)
                {
                    movie.Actors = GetActors(movie.Id);
                    return movie;
                }
            }

            string json;
            using (var wc = new WebClient())
            {
                wc.Encoding = Encoding.UTF8;
                json = wc.DownloadString(BASE_URL + "movie/" + id
                                         + "?api_key=" + API_KEY + "&language=en-US&append_to_response=videos");
            }
            var o = JObject.Parse(json);
            var m = GetMovieFromJObject(o);
            m.Actors = GetActors(id);
            return m;
        }

        private MovieViewModel GetMovieFromJObject(JObject o)
        {
            var movie = new MovieViewModel
            {
                Id = (int) o["id"],
                Title = (string) o["title"],
                Backdrop = "https://image.tmdb.org/t/p/w1280" + (string) o["backdrop_path"],
                Description = (string) o["overview"],
                ImdbID = (string)o["imdb_id"],
                Poster = "https://image.tmdb.org/t/p/w500" + (string)o["poster_path"],
                Rating = (double)o["vote_average"],
                ReleaseDate = (string)o["release_date"]
        };

            try
            {
                var genArray = JArray.FromObject(o["genres"]);
                foreach (var t in genArray)
                {
                    var genre = JObject.FromObject(t);
                    movie.Genres[(string) genre["name"]] = (int) genre["id"];
                }

                var trailerArray = JArray.FromObject(o["videos"]["results"]);
                foreach (var t in trailerArray)
                {
                    var trailer = JObject.FromObject(t);
                    if (!trailer["type"].ToString().Equals("Trailer")) continue;
                    movie.Trailer = "https://www.youtube.com/embed/" + trailer["key"] + "?autoplay=1";
                    break;
                }

            }
            catch (ArgumentNullException e)
            {
                Console.WriteLine(e);
            }

            try
            {
                movie.Runtime = (int) o["runtime"];
            }
            catch (ArgumentNullException e)
            {
                Console.WriteLine(movie.Title);
                Console.WriteLine(e);
            }
            movie.ProminentColor = Utils.Utils.GetDominantColor(
                GetBitmapFromUrl("https://image.tmdb.org/t/p/w92" +
                                 (string) o["poster_path"]));

            return movie;
        }

        private Dictionary<int, MovieViewModel> GetList(int amount, string type, string genre)
        {
            var list = new Dictionary<int, MovieViewModel>();
            var query =
                "&language=en-US&sort_by=" + type
                + "&include_adult=false&include_video=false&page=1&vote_count.gte=200&with_original_language=en";
            if (genre != null)
                query = query + "&with_genres=" + genre;

            string json;
            using (var wc = new WebClient())
            {
                json = wc.DownloadString(BASE_URL + "discover/movie"
                                         + "?api_key=" + API_KEY + query);
            }
            var o = JObject.Parse(json);
            var results = JArray.FromObject(o["results"]);

            for (var i = 0; i < amount && i < results.Count; i++)
            {
                var movie = JObject.FromObject(results[i]);
                var m = GetMovieFromJObject(movie);
                list[m.Id] = m;
            }
            return list;
        }

        private List<Actor> GetActors(int movieId)
        {
            var actorList = new List<Actor>();
            string json;
            using (var wc = new WebClient())
            {
                json = wc.DownloadString(BASE_URL + "movie/" + movieId + "/credits"
                                         + "?api_key=" + API_KEY);
            }
            var o = JObject.Parse(json);
            var actors = JArray.FromObject(o["cast"]);
            for (var i = 0; i < 5 && i < actors.Count; i++)
            {
                var actorObj = JObject.FromObject(actors[i]);
                var actor = new Actor();
                actor.Name = (string) actorObj["name"];
                actor.MovieName = (string) actorObj["character"];
                actor.Picture = "https://image.tmdb.org/t/p/w500" + (string) actorObj["profile_path"];
                actorList.Add(actor);
            }
            return actorList;
        }

        private void FetchGenres()
        {
            _genres = HttpContext.Current.Session["Genres"] as Dictionary<string, int>;
            if (_genres != null)
                return;
            _genres = new Dictionary<string, int>();
            string json;
            using (var wc = new WebClient())
            {
                json = wc.DownloadString(BASE_URL + "genre/movie/list?api_key=" + API_KEY + "&language=en-US");
            }
            var o = JObject.Parse(json);
            var results = JArray.FromObject(o["genres"]);

            foreach (var t in results)
            {
                var genre = JObject.FromObject(t);

                _genres[(string) genre["name"]] = (int) genre["id"];
            }
            HttpContext.Current.Session["Genres"] = _genres;
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
            var r = new Random();
            var index = r.Next(dvb.Popular.Count - 1);
            dvb.FeaturePopular = dvb.Popular.Values.ElementAt(index);
            dvb.Popular.Remove(dvb.FeaturePopular.Id);

            index = r.Next(dvb.TopRated.Count - 1);
            dvb.FeatureTopRated = dvb.TopRated.Values.ElementAt(index);
            dvb.TopRated.Remove(dvb.FeatureTopRated.Id);
        }

        public DiscoverViewModel GetDiscoverViewModel()
        {
            var discoverViewModel = HttpContext.Current.Session["Discover"] as DiscoverViewModel;
            if (discoverViewModel != null)
                return discoverViewModel;
            discoverViewModel = new DiscoverViewModel
            {
                Popular = GetList(21, "popularity.desc", null),
                TopRated = GetList(21, "vote_average.desc", null)
            };
            SetFeatureMovies(discoverViewModel);
            HttpContext.Current.Session["Discover"] = discoverViewModel;
            return discoverViewModel;
        }

        public GenreViewModel GetGenreViewModel(string genre)
        {
            var genreViewModel = HttpContext.Current.Session[genre] as GenreViewModel;
            if (genreViewModel != null)
                return genreViewModel;
            FetchGenres();
            genreViewModel = new GenreViewModel
            {
                GenreName = genre,
                GenreId = _genres[genre],
                Movies = GetList(20, "popularity.desc", _genres[genre].ToString())
            };
            HttpContext.Current.Session[genre] = genreViewModel;
            return genreViewModel;
        }
    }
}