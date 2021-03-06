﻿using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Net;
using System.Text;
using System.Web;
using Newtonsoft.Json.Linq;


/* Using TMDB as database for info about movies and tv shows.
 * Returns JSON
 * Read more here: https://www.themoviedb.org/documentation/api
 * https://developers.themoviedb.org/3
 */

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
            m.Actors = GetActors(id, "movie");
            return m;
        }

        public TvShow GetTvShow(int id)
        {
            var query = BASE_URL + "tv/" + id
                           + "?api_key=" + API_KEY + "&language=en-US";
            var json = GetJson(query);
            var o = JObject.Parse(json);
            var tv = GetTvShowFromJObject(o);
            tv.Actors = GetActors(id, "tv");
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
                ReleaseDate = (string)o["release_date"],
                ProminentColor = Utils.Utils.GetDominantColor(
                GetBitmapFromUrl("https://image.tmdb.org/t/p/w92" +
                                 (string)o["poster_path"]))
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
                EpisodeRuntime = (int)(o["episode_run_time"][0]),
                ProminentColor = Utils.Utils.GetDominantColor(
                GetBitmapFromUrl("https://image.tmdb.org/t/p/w92" +
                                 (string)o["poster_path"]))
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

        private PreviewItem GetPreviewItemFromJObject(JObject o)
        {
            bool isAMovie = o["title"] != null;
            if (o["known_for"] != null) return null; //Object is a person, which we do not handle. Return null

            var item = new PreviewItem
            {
                Id = (int)o["id"],
                Title = (string)o["title"],
                Backdrop = "https://image.tmdb.org/t/p/w780" + (string)o["backdrop_path"],
                BackdropHighRes = "https://image.tmdb.org/t/p/original" + (string)o["backdrop_path"],
                Poster = "https://image.tmdb.org/t/p/w500" + (string)o["poster_path"],
                VoteCount = (int)o["vote_count"],
                Description = (string)o["overview"],
                IsAMovie = isAMovie
            };

            if (isAMovie) //Movie properties
            {
                item.ReleaseDate = (string)o["release_date"];
                item.Title = (string) o["title"];
            }
            else  //TV Show properties
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
            while ((list.Count) < amount)
            {
                var query =
                    "&language=en-US&sort_by=" + sortBy
                    + "&include_adult=false&include_video=false&page=" + page +
                    "&vote_count.gte=100&with_original_language=en";
                if (genre != null)
                    query = query + "&with_genres=" + genre;

                var fullQuery = BASE_URL + "discover/movie"
                                   + "?api_key=" + API_KEY + query;
                var json = GetJson(fullQuery);

                var movies = ParseJsonArray(json);
                if (movies.Count == 0) return list;
                AddOnlyPopularItems(list, movies, amount);
                page++;
            }
            return list;
        }


        public Dictionary<int, PreviewItem> GetTVList(int amount, string sortBy)
        {
            var list = new Dictionary<int, PreviewItem>();
            int page = 1;
            while ((list.Count) < amount)
            {
                var query = "&language=en-US&page=" + page;

                var fullQuery = BASE_URL + "tv/" + sortBy
                                   + "?api_key=" + API_KEY + query;
                var json = GetJson(fullQuery);

                var shows = ParseJsonArray(json);
                if (shows.Count == 0) return list;
                AddOnlyPopularItems(list, shows, amount);
                page++;
            }
            return list;
        }

        public Dictionary<int, PreviewItem> GetSimilarMoviesList(int id, string videoType, bool isAMovie)
        {
            var list = new Dictionary<int, PreviewItem>();
            var page = 1;

            while (list.Count < 20)
            {
                var query = BASE_URL + videoType + "/" + id + "/similar"
                               + "?api_key=" + API_KEY + "&language=en-US&page=" + page;
                var json = GetJson(query);
                var fetchedList = ParseJsonArray(json);
                if (fetchedList.Count == 0) return list;
                AddOnlyPopularItems(list, fetchedList, 20);
                page++; 
            }
            return list;
        }

        public Dictionary<int, PreviewItem> GetSearchResultList(string searchResult)
        {
            var list = new Dictionary<int, PreviewItem>();

            var query = BASE_URL + "search/multi?api_key=" + API_KEY + "&language=en-US&query=" + searchResult
                + "&include_adult=false";

            var json = GetJson(query);
            var fetchedList = ParseJsonArray(json);

            foreach (var item in fetchedList)
            {
                if (!item.Value.Poster.Equals("https://image.tmdb.org/t/p/w500"))
                {
                    list.Add(item.Key, item.Value);
                }
            }
            return list;

        }

        private void AddOnlyPopularItems(Dictionary<int, PreviewItem> listToShow,
            Dictionary<int, PreviewItem> fetchedList, int amount)
        {
            foreach (var item in fetchedList)
            {
                if ((listToShow.Count) >= amount) { return; }
                try
                {

                    if (item.Value.VoteCount > 100 && !item.Value.Poster.Equals("https://image.tmdb.org/t/p/w500"))
                    {
                        listToShow.Add(item.Key, item.Value);
                    }
                }
                catch (ArgumentException e)
                {
                    Console.WriteLine(e);
                }
            }
        }

        private Dictionary<int, PreviewItem> ParseJsonArray(string json)
        {
            var list = new Dictionary<int, PreviewItem>();

            var o = JObject.Parse(json);
            var results = JArray.FromObject(o["results"]);

            foreach (var t in results)
            {
                var item = JObject.FromObject(t);
                var m = GetPreviewItemFromJObject(item);
                if (m != null)
                {
                    list[m.Id] = m;
                }
            }
            return list;
        }

        private List<Actor> GetActors(int id, string videoType)
        {
            var actorList = new List<Actor>();
            var query = BASE_URL + videoType + "/" + id + "/credits"
                           + "?api_key=" + API_KEY;
            var json = GetJson(query);
            var o = JObject.Parse(json);
            var actors = JArray.FromObject(o["cast"]);
            int i = 0;
            while (actorList.Count <= 5 && i < actors.Count)
            {
                var actorObj = JObject.FromObject(actors[i]);
                if ((int)actorObj["order"] <= 6)
                {
                    var actor = new Actor
                    {
                        Name = (string)actorObj["name"],
                        CharacterName = (string)actorObj["character"],
                        Picture = "https://image.tmdb.org/t/p/w185" + (string)actorObj["profile_path"]
                    };
                    actorList.Add(actor); 
                }
                i++;
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