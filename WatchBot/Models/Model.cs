using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using WatchBot.Models.ViewModels;

namespace WatchBot.Models
{
    public class Model
    {

        public DiscoverViewModel GetDiscoverViewModel()
        {
            var discoverViewModel = HttpContext.Current.Session["Discover"] as DiscoverViewModel;
            if (discoverViewModel != null)
                return discoverViewModel;
            discoverViewModel = new DiscoverViewModel();
            discoverViewModel.Carousels.Add(new CarouselViewModel
            {
                CarouselName = "Popular",
                Items = DBWrapper.Instance.GetMovieList(21, "popularity.desc", null)
            });
            SetFeatureItem(discoverViewModel.Carousels.ElementAt(0));

            discoverViewModel.Carousels.Add(new CarouselViewModel
            {
                CarouselName = "Top Rated",
                Items = DBWrapper.Instance.GetMovieList(21, "vote_average.desc", null)
            });
            SetFeatureItem(discoverViewModel.Carousels.ElementAt(1));

            HttpContext.Current.Session["Discover"] = discoverViewModel;
            return discoverViewModel;
        }

        public TVShowsViewModel GetTvShowsViewModel()
        {
            var TVModel = HttpContext.Current.Session["TVShows"] as TVShowsViewModel;
            if (TVModel != null)
                return TVModel;
            TVModel = new TVShowsViewModel();
            TVModel.Carousels.Add(new CarouselViewModel
            {
                CarouselName = "On The Air",
                Items = DBWrapper.Instance.GetTVList(21, "on_the_air")
            });
            SetFeatureItem(TVModel.Carousels.ElementAt(0));

            TVModel.Carousels.Add(new CarouselViewModel
            {
                CarouselName = "Popular",
                Items = DBWrapper.Instance.GetTVList(21, "popular")
            });
            SetFeatureItem(TVModel.Carousels.ElementAt(1));

            TVModel.Carousels.Add(new CarouselViewModel
            {
                CarouselName = "Top Rated",
                Items = DBWrapper.Instance.GetTVList(21, "top_rated")
            });
            SetFeatureItem(TVModel.Carousels.ElementAt(2));

            HttpContext.Current.Session["TVShows"] = TVModel;
            return TVModel;
        }

        public GenreViewModel GetGenreViewModel(string genre, int page)
        {
            var genreViewModel = HttpContext.Current.Session[genre] as GenreViewModel;
            if (genreViewModel != null)
                return genreViewModel;

            genreViewModel = new GenreViewModel
            {
                GenreName = genre,
                GenreId = DBWrapper.Instance._genres[genre],
                Movies = DBWrapper.Instance.GetMovieList(60, "popularity.desc", DBWrapper.Instance._genres[genre].ToString())
            };
            HttpContext.Current.Session[genre] = genreViewModel;
            return genreViewModel;
        }

        public MovieInfoViewModel GetMovieInfoViewModel(string movieId)
        {
            var id = int.Parse(movieId);
            var movieInfoViewModel = new MovieInfoViewModel
            {
                Movie = DBWrapper.Instance.GetMovie(id),
                Similar = new CarouselViewModel
                {
                    CarouselName = "Similar Movies",
                    Items = DBWrapper.Instance.GetSimilarMoviesList(id, "movie", true)
                }
            };
            return movieInfoViewModel;
        }

        private void SetFeatureItem(CarouselViewModel cm)
        {
            if (cm == null) return;
            var r = new Random();
            var index = r.Next(cm.Items.Count - 1);
            cm.Highlight = cm.Items.Values.ElementAt(index);
            cm.Items.Remove(cm.Highlight.Id);
        }

        public TvShowInfoViewModel GeTvShowInfoViewModel(string tvShowId)
        {
            var id = int.Parse(tvShowId);
            var tvShowInfo = new TvShowInfoViewModel()
            {
                TvShow = DBWrapper.Instance.GetTvShow(id),
                Similar = new CarouselViewModel
                {
                    CarouselName = "Similar Tv Shows",
                    Items = DBWrapper.Instance.GetSimilarMoviesList(id, "tv", false)
                }
            };
            return tvShowInfo;
        }

        public SearchResultViewModel GetSearchResultViewModel(string searchString)
        {
            var searchResult = new SearchResultViewModel
            {
                Items = DBWrapper.Instance.GetSearchResultList(searchString),
                SearchString = searchString
            };
            return searchResult;
        }
    }
}