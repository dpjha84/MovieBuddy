using System;
using System.Collections.Generic;
using System.Linq;
using TMDbLib.Client;
using TMDbLib.Objects.Movies;
using TMDbLib.Objects.People;
using TMDbLib.Objects.Trending;

namespace MovieBuddy
{
    public class MovieManager
    {
        public static void Init(ILocalDataProvider provider, bool fake = false)
        {
            if (Instance == null)
            {
                Instance = new MovieManager(provider);                
                Instance.Load();
            }
        }

        TMDbClient tmdbClient;
        public static MovieManager Instance { get; private set; }

        public ILocalDataProvider LocalDataProvider;
        public Dictionary<string, TmdbMovie> MovieData = new Dictionary<string, TmdbMovie>();

        private MovieManager(ILocalDataProvider provider)
        {
            LocalDataProvider = provider;
            tmdbClient = new TMDbClient("c6b31d1cdad6a56a23f0c913e2482a31");
        }

        public void Load()
        {
            Upcoming = tmdbClient.GetMovieUpcomingListAsync().Result.Results;
            NowPlaying = tmdbClient.GetMovieNowPlayingListAsync().Result.Results;
            Trending = tmdbClient.GetTrendingMoviesAsync(TimeWindow.Week).Result.Results;
            //..Select(x => new TmdbMovie
            //{
            //    Id = x.Id,
            //    Title = x.Title,
            //    PosterPath = $"https://image.tmdb.org/t/p/w600_and_h900_bestv2/{x.PosterPath}",
            //    BackdropPath = $"https://image.tmdb.org/t/p/w600_and_h900_bestv2/{x.BackdropPath}",
            //    Overview = x.Overview
            //}).ToList();
            //NowPlaying = tmdbClient.GetMovieNowPlayingListAsync().Result.Results.Select(x => new TmdbMovie
            //{
            //    Id = x.Id,
            //    Title = x.Title,
            //    PosterPath = $"https://image.tmdb.org/t/p/w600_and_h900_bestv2/{x.PosterPath}",
            //    BackdropPath = $"https://image.tmdb.org/t/p/w600_and_h900_bestv2/{x.BackdropPath}",
            //    Overview = x.Overview
            //}).ToList();
            //Trending = tmdbClient.GetTrendingMoviesAsync(TimeWindow.Week).Result.Results.Select(x => new TmdbMovie
            //{
            //    Id = x.Id,
            //    Title = x.Title,
            //    PosterPath = $"https://image.tmdb.org/t/p/w600_and_h900_bestv2/{x.PosterPath}",
            //    BackdropPath = $"https://image.tmdb.org/t/p/w600_and_h900_bestv2/{x.BackdropPath}",
            //    Overview = x.Overview
            //}).ToList();
        }

        public List<TMDbLib.Objects.Search.SearchMovie> Upcoming { get; private set; }
        public List<TMDbLib.Objects.Search.SearchMovie> NowPlaying { get; private set; }
        public List<TMDbLib.Objects.Search.SearchMovie> Trending { get; private set; }

        public string GetTrailer(int movieId, string name) => "";

        public List<TmdbMovie> GetReleased()
        {
            var newList = MovieData.Values.Where(c => c.ReleaseDate <= DateTime.Now).ToList();
            return newList.OrderByDescending(m => m.ReleaseDate).ToList();
        }

        public List<TMDbLib.Objects.Search.SearchMovie> GetAllTrailers()
        {
            //var list = MovieData.Values.Where(c => !string.IsNullOrWhiteSpace(c.Trailer) && c.ReleaseDate > DateTime.Now).ToList();
            //return list.OrderBy(m => m.ReleaseDate).ToList();
            return new List<TMDbLib.Objects.Search.SearchMovie>();
        }

        public bool IsMoviePresent(string movieName)
        {
            var movie = MovieData.Values.Where(m => m.OriginalTitle == movieName).FirstOrDefault();
            return movie != null;
        }

        public Credits GetCast(int movieId, string name)
        {
            return new Credits { Cast = new List<Cast>() };
            //var data = MovieData[GetKey(movieId, name)].Credits;
            //if (data == null)
            //    return new Credits { Cast = new List<Cast>() };
            //return data;
        }

        public TMDbLib.Objects.Movies.Credits GetCastAndCrew(int movieId) => tmdbClient.GetMovieCreditsAsync(movieId).Result;

        public List<TMDbLib.Objects.Search.SearchMovie> GetSimilar(int movieId) => tmdbClient.GetMovieSimilarAsync(movieId).Result.Results;

        public List<TMDbLib.Objects.Reviews.ReviewBase> GetReviews(int movieId) => tmdbClient.GetMovieReviewsAsync(movieId).Result.Results;

        public MovieCredits GetMovieCredits(int personId)
        {
            return tmdbClient.GetPersonMovieCreditsAsync(personId).Result;
        }

        public string GetGenres(TmdbMovie movie, int count = 0)
        {
            var genres = movie.GenreText;
            if (count == 0 || string.IsNullOrWhiteSpace(genres))
                return genres;
            var splits = genres.Split(new string[] { ", " }, StringSplitOptions.RemoveEmptyEntries).Take(count);
            var join = string.Join(", ", splits);
            return join;
        }

        public string GetFullOverview(int movieId)
        {
            var movie = Upcoming.Find(x => x.Id == movieId);
            if (movie != null) return movie.Overview;
            movie = NowPlaying.Find(x => x.Id == movieId);
            if (movie != null) return movie.Overview;
            movie = Trending.Find(x => x.Id == movieId);
            if (movie != null) return movie.Overview;
            return "";
        }
    }
}
