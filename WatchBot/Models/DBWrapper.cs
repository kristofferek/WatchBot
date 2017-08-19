using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Net;
using System.Text;
using System.Web;
using Newtonsoft.Json.Linq;

namespace WatchBot.Models
{
    public class DBWrapper
    {
        private static DBWrapper instance;

        private DBWrapper() { FetchGenres();}

        public static DBWrapper Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = new DBWrapper();
                }
                return instance;
            }
        }
        private const string API_KEY = "78a3dcb0adbc04e0efe7af4db390f544";
        private const string BASE_URL = "https://api.themoviedb.org/3/";



        public Dictionary<string, int> _genres;

        public Movie GetMovie(int id)
        {
            var query = BASE_URL + "movie/" + id
                           + "?api_key=" + API_KEY + "&language=en-US&append_to_response=videos";
            var json = GetJson(query);
            var o = JObject.Parse(json);
            var m = GetMovieFromJObject(o);
            m.Actors = GetActors(id);
            return m;
        }

        public TvShow GetTvShow(int id)
        {
            var query = BASE_URL + "tv/" + id
                           + "?api_key=" + API_KEY + "&language=en-US";
            var json = GetJson(query);
            var o = JObject.Parse(json);
            var tv = GetTvShowFromJObject(o);
            return tv;
        }

        private Movie GetMovieFromJObject(JObject o)
        {
            var movie = new Movie
            {
                Id = (int) o["id"],
                Title = (string) o["title"],
                Backdrop = "https://image.tmdb.org/t/p/w1280" + (string) o["backdrop_path"],
                Description = (string) o["overview"],
                Poster = "https://image.tmdb.org/t/p/w500" + (string)o["poster_path"],
                Rating = ((double)o["vote_average"])*10 ,
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

                var videosToken = o["videos"];
                if (videosToken != null)
                {
                    var trailerArray = JArray.FromObject(videosToken["results"]);
                    foreach (var t in trailerArray)
                    {
                        var trailer = JObject.FromObject(t);
                        movie.Trailer = "https://www.youtube.com/embed/" + trailer["key"] + "?autoplay=1";
                        if (!trailer["type"].ToString().Equals("Trailer")) continue;
                        break;
                    }
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

        private TvShow GetTvShowFromJObject(JObject o)
        {
            var tvShow = new TvShow
            {
                Id = (int)o["id"],
                Title = (string)o["name"],
                Backdrop = "https://image.tmdb.org/t/p/w1280" + (string)o["backdrop_path"],
                Description = (string)o["overview"],
                Poster = "https://image.tmdb.org/t/p/w500" + (string)o["poster_path"],
                Rating = ((double)o["vote_average"]) * 10,
                FirstAirDate = (string)o["first_air_date"],
                EpisodeRuntime = (int)(o["episode_run_time"][0])
            };

            var networks = JArray.FromObject(o["networks"]);
            tvShow.Network = (string)(JObject.FromObject(networks.First))["name"];

            var genArray = JArray.FromObject(o["genres"]);
            foreach (var t in genArray)
            {
                var genre = JObject.FromObject(t);
                tvShow.Genres[(string)genre["name"]] = (int)genre["id"];
            }

            var seasons = JArray.FromObject(o["seasons"]);
            foreach (var s in seasons)
            {
                var season = JObject.FromObject(s);
                tvShow.Seasons.Add((int)season["id"], new TvSeason
                {
                    FirstAirDate = (string)season["air_date"],
                    Id = (int)season["id"],
                    NumberOfEpisodes = (int)season["episode_count"],
                    Poster = (string)season["poster_path"],
                    SeasonNumber = (int)season["season_number"]
                });
            }

            return tvShow;
        }

        private PreviewItem GetPreviewItemFromJObject(JObject o, bool isAMovie)
        {
            var item = new PreviewItem
            {
                Id = (int)o["id"],
                Title = (string)o["title"],
                Backdrop = "https://image.tmdb.org/t/p/w1280" + (string)o["backdrop_path"],
                Poster = "https://image.tmdb.org/t/p/w500" + (string)o["poster_path"],
                VoteCount = (int)o["vote_count"],
                isAMovie = isAMovie
            };

            if (isAMovie)
            {
                item.ReleaseDate = (string)o["release_date"];
                item.Title = (string) o["title"];
            }
            else  //Is a TV Show
            {
                item.ReleaseDate = (string)o["first_air_date"];
                item.Title = (string)o["name"];
            }

            return item;
        }

        public Dictionary<int, PreviewItem> GetMovieList(int amount, string sortBy, string genre)
        {
            var list = new Dictionary<int, PreviewItem>();
            int page = 1;
            while ((list.Count + 1) <= amount)
            {
                var query =
                    "&language=en-US&sort_by=" + sortBy
                    + "&include_adult=false&include_video=false&page=" + page +
                    "&vote_count.gte=200&with_original_language=en";
                if (genre != null)
                    query = query + "&with_genres=" + genre;

                var fullQuery = BASE_URL + "discover/movie"
                                   + "?api_key=" + API_KEY + query;
                var json = GetJson(fullQuery);

                var movies = ParseJsonArray(json, true);
                if (movies.Count == 0) return list;

                foreach (var movie in movies)
                {
                    if ((list.Count + 1) >= amount) { return list; }
                    try
                    {
                        list.Add(movie.Key, movie.Value);
                    }
                    catch (ArgumentException e)
                    {
                        Console.WriteLine(e);
                    }
                }
                page++;
            }
            return list;
        }

        public Dictionary<int, PreviewItem> GetTVList(int amount, string sortBy)
        {
            var list = new Dictionary<int, PreviewItem>();
            int page = 1;
            while ((list.Count + 1) <= amount)
            {
                var query = "&language=en-US&page=" + page;

                var fullQuery = BASE_URL + "tv/" + sortBy
                                   + "?api_key=" + API_KEY + query;
                var json = GetJson(fullQuery);

                var shows = ParseJsonArray(json, false);
                if (shows.Count == 0) return list;

                foreach (var show in shows)
                {
                    if ((list.Count + 1) >= amount) { return list; }
                    try
                    {

                        if (show.Value.VoteCount > 100)
                        {
                            list.Add(show.Key, show.Value); 
                        }
                    }
                    catch (ArgumentException e)
                    {
                        Console.WriteLine(e);
                    }
                }
                page++;
            }
            return list;
        }

        public Dictionary<int, PreviewItem> GetSimilarMoviesList(int id, string type)
        {
            var query = BASE_URL + "movie/" + id + "/" + type
                           + "?api_key=" + API_KEY + "&language=en-US&page=1";
            var json = GetJson(query);
            return ParseJsonArray(json, true);
        }

        private Dictionary<int, PreviewItem> ParseJsonArray(string json, bool isAMovie)
        {
            var list = new Dictionary<int, PreviewItem>();

            var o = JObject.Parse(json);
            var results = JArray.FromObject(o["results"]);

            foreach (var t in results)
            {
                var movie = JObject.FromObject(t);
                var m = GetPreviewItemFromJObject(movie, isAMovie);
                list[m.Id] = m;
            }
            return list;
        }

        private List<Actor> GetActors(int movieId)
        {
            var actorList = new List<Actor>();
            var query = BASE_URL + "movie/" + movieId + "/credits"
                           + "?api_key=" + API_KEY;
            var json = GetJson(query);
            var o = JObject.Parse(json);
            var actors = JArray.FromObject(o["cast"]);
            for (var i = 0; i < 5 && i < actors.Count; i++)
            {
                var actorObj = JObject.FromObject(actors[i]);
                var actor = new Actor
                {
                    Name = (string) actorObj["name"],
                    MovieName = (string) actorObj["character"],
                    Picture = "https://image.tmdb.org/t/p/w500" + (string) actorObj["profile_path"]
                };
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
            const string query = BASE_URL + "genre/movie/list?api_key=" + API_KEY + "&language=en-US";
            var json = GetJson(query);
            var o = JObject.Parse(json);
            var results = JArray.FromObject(o["genres"]);

            foreach (var t in results)
            {
                var genre = JObject.FromObject(t);

                _genres[(string) genre["name"]] = (int) genre["id"];
            }
            HttpContext.Current.Session["Genres"] = _genres;
        }

        private string GetJson(string query)
        {
            string json;
            using (var wc = new WebClient())
            {
                wc.Encoding = Encoding.UTF8;
                try
                {
                    json = wc.DownloadString(query);
                }
                catch (Exception)
                {

                    return null;
                }
            }
            return json;
        }

        private static Bitmap GetBitmapFromUrl(string src)
        {
            var request = WebRequest.Create(src);
            var response = request.GetResponse();
            var responseStream = response.GetResponseStream();
            var bitmap = new Bitmap(responseStream);
            return bitmap;
        }
    }
}